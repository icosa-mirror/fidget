#!/usr/bin/env python3
"""Build and package the native probe. Run from any directory; no third-party Python deps."""

import argparse
import os
from pathlib import Path
import re
import shlex
import shutil
import subprocess
import sys


ROOT = Path(__file__).resolve().parent.parent
TARGETS = {
    "x86_64-pc-windows-msvc": "fidget_native.dll",
    "aarch64-apple-darwin": "libfidget_native.dylib",
    "x86_64-apple-darwin": "libfidget_native.dylib",
    "x86_64-unknown-linux-gnu": "libfidget_native.so",
    "aarch64-apple-ios": "libfidget_native.a",
    "aarch64-apple-ios-sim": "libfidget_native.a",
    "aarch64-linux-android": "libfidget_native.so",
    "x86_64-linux-android": "libfidget_native.so",
}
SYMBOLS = (
    "fidget_native_abi_version",
    "fidget_mesh_sphere",
    "fidget_mesh_get_counts",
    "fidget_mesh_copy_to",
    "fidget_mesh_free",
    "fidget_shape_compile",
    "fidget_shape_free",
    "fidget_mesher_create",
    "fidget_mesher_generate",
    "fidget_mesher_generate_with_budget",
    "fidget_mesher_free",
    "fidget_mesh_write_normals",
    "fidget_mesh_prepare_surface_normals",
)


def run(args, env):
    print(f"+ {shlex.join(str(a) for a in args)}", flush=True)
    subprocess.run(args, cwd=ROOT, env=env, check=True)


def capture(args, env):
    return subprocess.check_output(args, cwd=ROOT, env=env, text=True)


def android_tools(target, ndk, env):
    if not ndk:
        raise ValueError("Set --android-ndk or ANDROID_NDK_HOME to an installed NDK r28 or newer")
    ndk = Path(ndk).resolve()
    properties = (ndk / "source.properties").read_text()
    revision = re.search(r"Pkg.Revision\s*=\s*(\d+)[^\r\n]*", properties)
    if not revision or int(revision[1]) < 28:
        raise ValueError("Use Android NDK r28 or newer for 16 KB page-size support")
    host_tag = {"win32": "windows-x86_64", "linux": "linux-x86_64", "darwin": "darwin-x86_64"}[sys.platform]
    binaries = ndk / "toolchains" / "llvm" / "prebuilt" / host_tag / "bin"
    suffix = ".cmd" if sys.platform == "win32" else ""
    linker = binaries / f"{target}23-clang{suffix}"
    if not linker.is_file():
        raise ValueError(f"Missing Android linker: {linker}")
    cargo_key = target.upper().replace("-", "_")
    env[f"CARGO_TARGET_{cargo_key}_LINKER"] = str(linker)
    # Explicitly align both LOAD segments and RELRO boundaries on 16 KB systems.
    # Verification below also catches caller-supplied RUSTFLAGS overriding this.
    env[f"CARGO_TARGET_{cargo_key}_RUSTFLAGS"] = (
        "-C link-arg=-Wl,-z,max-page-size=16384 "
        "-C link-arg=-Wl,-z,common-page-size=16384"
    )
    return binaries, revision[0]


def check_android(library, binaries, env):
    suffix = ".exe" if sys.platform == "win32" else ""
    headers = capture([str(binaries / f"llvm-readelf{suffix}"), "-lW", str(library)], env)
    loads = [line.split() for line in headers.splitlines() if line.strip().startswith("LOAD ")]
    if not loads or any(int(line[-1], 16) < 16384 for line in loads):
        raise ValueError("Android ELF LOAD alignment is smaller than 16 KB")
    for line in loads:
        if int(line[1], 16) % 16384 != int(line[2], 16) % 16384:
            raise ValueError("Android ELF LOAD file/virtual offsets are not congruent at 16 KB")
    exports = capture([str(binaries / f"llvm-nm{suffix}"), "--dynamic", "--defined-only", str(library)], env)
    names = {line.split()[-1] for line in exports.splitlines() if line.split()}
    if not set(SYMBOLS).issubset(names):
        raise ValueError("Android library is missing a public C ABI symbol")
    print("Android ELF: 16 KB LOAD alignment and all public C exports verified", flush=True)


def link_ios_probe(target, library, native_libs, build_dir, env):
    simulator = target.endswith("-sim")
    sdk = "iphonesimulator" if simulator else "iphoneos"
    sdk_path = capture(["xcrun", "--sdk", sdk, "--show-sdk-path"], env).strip()
    deployment = env["IPHONEOS_DEPLOYMENT_TARGET"]
    clang_target = f"arm64-apple-ios{deployment}{'-simulator' if simulator else ''}"
    run([
        "xcrun", "--sdk", sdk, "clang", "-target", clang_target,
        "-isysroot", sdk_path, "-std=c99",
        "-I", str(ROOT / "fidget-native/include"),
        str(ROOT / "fidget-native/tests/smoke.c"), str(library),
        *shlex.split(native_libs), "-o", str(build_dir / "fidget-native-link-probe"),
    ], env)
    print("iOS C probe linked (not executed)", flush=True)


def desktop_smoke(target, library, env):
    build_dir = ROOT / "target/native-smoke" / target
    configure = ["cmake", "-S", str(ROOT / "fidget-native/tests"), "-B", str(build_dir),
                 "-DCMAKE_BUILD_TYPE=Release"]
    if target.endswith("windows-msvc"):
        configure += [f"-DFIDGET_LIBRARY={library.with_suffix('.dll.lib')}",
                      f"-DFIDGET_RUNTIME={library}"]
    else:
        configure += [f"-DFIDGET_LIBRARY={library}"]
        path_key = "DYLD_LIBRARY_PATH" if "apple" in target else "LD_LIBRARY_PATH"
        env[path_key] = os.pathsep.join(filter(None, [str(library.parent), env.get(path_key)]))
    run(configure, env)
    run(["cmake", "--build", str(build_dir), "--config", "Release"], env)
    run(["ctest", "--test-dir", str(build_dir), "-C", "Release", "--output-on-failure"], env)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--target", required=True, choices=TARGETS)
    parser.add_argument("--android-ndk", default=os.environ.get("ANDROID_NDK_HOME") or os.environ.get("ANDROID_NDK_ROOT"))
    parser.add_argument("--smoke", action="store_true", help="Run the C ABI smoke test; requires a matching desktop host")
    args = parser.parse_args()
    env = os.environ.copy()
    env["CARGO_TERM_COLOR"] = "never"
    rust_version = capture(["rustc", "-vV"], env)
    if args.smoke and f"host: {args.target}\n" not in rust_version:
        raise ValueError("--smoke requires a matching desktop Rust host")
    ios = "apple-ios" in args.target
    ndk_info = ""
    if "android" in args.target:
        binaries, ndk_info = android_tools(args.target, args.android_ndk, env)
    elif ios:
        env.setdefault("IPHONEOS_DEPLOYMENT_TARGET", "13.0")
    elif "apple-darwin" in args.target:
        env.setdefault("MACOSX_DEPLOYMENT_TARGET", "11.0")

    # Guard against accidentally introducing JIT into the portable baseline.
    tree = capture(["cargo", "tree", "--locked", "-p", "fidget-native", "--target", args.target,
                    "--edges", "normal", "--prefix", "none"], env)
    if any(line.startswith("fidget-jit ") for line in tree.splitlines()):
        raise ValueError("The native baseline must not depend on fidget-jit")

    build_root = ROOT / "target/native-build"
    build = ["cargo", "rustc", "--locked", "-p", "fidget-native", "--lib", "--release",
             "--target", args.target, "--target-dir", str(build_root),
             "--crate-type", "staticlib" if ios else "cdylib"]
    native_libs = ""
    if ios:
        # Record rustc's actual native link requirements, rather than guessing
        # which libraries the Apple Rust standard library needs.
        result = subprocess.run(build + ["--", "--print", "native-static-libs"],
                                cwd=ROOT, env=env, text=True, capture_output=True)
        print(result.stdout, end="")
        print(result.stderr, end="", file=sys.stderr)
        result.check_returncode()
        match = re.search(r"native-static-libs: ([^\r\n]+)", result.stderr)
        if not match:
            raise ValueError("rustc did not report the iOS static library link requirements")
        native_libs = match[1]
    else:
        run(build, env)

    build_dir = build_root / args.target / "release"
    library = build_dir / TARGETS[args.target]
    if "android" in args.target:
        check_android(library, binaries, env)
    elif ios:
        link_ios_probe(args.target, library, native_libs, build_dir, env)
    if args.smoke:
        desktop_smoke(args.target, library, env)

    package = ROOT / "target/native-artifacts" / args.target
    package.mkdir(parents=True, exist_ok=True)
    for source in [library, ROOT / "fidget-native/include/fidget_native.h",
                   ROOT / "LICENSE.txt", ROOT / "fidget-native/README.md"]:
        shutil.copy2(source, package / source.name)
    if args.target.endswith("windows-msvc"):
        shutil.copy2(library.with_suffix(".dll.lib"), package / "fidget_native.dll.lib")
    commit = capture(["git", "rev-parse", "HEAD"], env).strip()
    dirty = bool(capture(["git", "status", "--porcelain", "--untracked-files=normal"], env).strip())
    (package / "BUILD-INFO.txt").write_text(
        f"ABI: 7\nTarget: {args.target}\nSource commit: {commit}\nWorking tree dirty: {dirty}\n"
        f"{rust_version}{ndk_info}\nNative static libs: {native_libs}\n"
        f"Desktop C smoke executed: {args.smoke}\niOS C probe linked: {ios}\n",
        encoding="utf-8",
    )
    print(f"Packaged: {package}", flush=True)


if __name__ == "__main__":
    try:
        main()
    except (OSError, ValueError, subprocess.CalledProcessError) as error:
        sys.exit(f"Native build failed: {error}")
