#!/usr/bin/env python3
"""Assemble a local UPM package from source and ABI-v3 native build artifacts."""
import argparse
from pathlib import Path
import shutil
import uuid

ROOT = Path(__file__).resolve().parent.parent
PLATFORMS = {
    "x86_64-pc-windows-msvc": ("Windows/x86_64", "fidget_native.dll", "Standalone", "Win64", "x86_64", "Windows"),
    "aarch64-apple-darwin": ("macOS/ARM64", "libfidget_native.dylib", "Standalone", "OSXUniversal", "ARM64", "OSX"),
    "x86_64-apple-darwin": ("macOS/x86_64", "libfidget_native.dylib", "Standalone", "OSXUniversal", "x86_64", "OSX"),
    "x86_64-unknown-linux-gnu": ("Linux/x86_64", "libfidget_native.so", "Standalone", "Linux64", "x86_64", "Linux"),
    "aarch64-linux-android": ("Android/ARM64", "libfidget_native.so", "Android", "Android", "ARM64", None),
    "x86_64-linux-android": ("Android/x86_64", "libfidget_native.so", "Android", "Android", "x86_64", None),
    "aarch64-apple-ios": ("iOS", "libfidget_native.a", "iPhone", "iOS", "AnyCPU", None),
    "aarch64-apple-ios-sim": ("iOS", "libfidget_native.a", "iPhone", "iOS", "AnyCPU", None),
}


def guid(path):
    return uuid.uuid5(uuid.NAMESPACE_URL, f"com.icosa.fidget/{path}").hex


def plugin_meta(path, platform):
    _, _, group, target, cpu, editor_os = platform
    editor = 1 if editor_os else 0
    return f"""fileFormatVersion: 2
guid: {guid(path)}
PluginImporter:
  serializedVersion: 2
  isPreloaded: 0
  isOverridable: 0
  isExplicitlyReferenced: 0
  validateReferences: 1
  platformData:
  - first:
      Any:
    second:
      enabled: 0
      settings: {{}}
  - first:
      Editor: Editor
    second:
      enabled: {editor}
      settings:
        CPU: {cpu if editor else 'AnyCPU'}
        OS: {editor_os or 'AnyOS'}
  - first:
      {group}: {target}
    second:
      enabled: 1
      settings:
        CPU: {cpu}
"""


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--artifacts", type=Path, required=True)
    parser.add_argument("--output", type=Path, default=ROOT / "target/unity-package/com.icosa.fidget")
    parser.add_argument("--targets", nargs="+", choices=PLATFORMS, required=True)
    args = parser.parse_args()
    if "aarch64-apple-ios" in args.targets and "aarch64-apple-ios-sim" in args.targets:
        parser.error("Select either iOS device or simulator; never link both ARM64 archives")
    if args.output.exists():
        parser.error("Output already exists; choose a new destination to avoid stale native plugins")
    sources = []
    for target in args.targets:
        source = args.artifacts / target
        if not source.is_dir():
            source = args.artifacts / f"fidget-native-{target}"
        info = (source / "BUILD-INFO.txt").read_text()
        if "ABI: 3\n" not in info:
            parser.error(f"{target} needs rebuilt ABI-v3 artifacts; phase-1 binaries are incompatible")
        if not (source / PLATFORMS[target][1]).is_file():
            parser.error(f"Missing library for {target}")
        sources.append((target, source))
    shutil.copytree(ROOT / "unity/com.icosa.fidget", args.output)
    shutil.copy2(ROOT / "LICENSE.txt", args.output / "LICENSE.md")
    for target, source in sources:
        platform = PLATFORMS[target]
        relative = Path("Runtime/Plugins") / platform[0] / platform[1]
        destination = args.output / relative
        destination.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(source / platform[1], destination)
        shutil.copy2(source / "BUILD-INFO.txt", destination.parent / "BUILD-INFO.txt")
        destination.with_name(f"{destination.name}.meta").write_text(plugin_meta(relative.as_posix(), platform))
    # Stable identities across package rebuilds. Unity fills in importer defaults.
    for path in list(args.output.rglob("*")):
        if path.name.endswith(".meta"):
            continue
        meta = path.with_name(f"{path.name}.meta")
        if not meta.exists():
            folder = "folderAsset: yes\n" if path.is_dir() else ""
            meta.write_text(f"fileFormatVersion: 2\nguid: {guid(path.relative_to(args.output).as_posix())}\n{folder}")
    print(f"Packaged {len(sources)} target(s): {args.output.resolve()}")


if __name__ == "__main__":
    main()
