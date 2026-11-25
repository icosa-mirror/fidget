# Assembling the Unity package

Applications consume the complete package through Unity Package Manager:

```json
"com.icosa.fidget": "https://github.com/icosa-mirror/fidget.git#upm"
```

The `upm` branch contains both C# and native libraries. Use `upm-simulator` for
macOS plus ARM64 iOS simulator libraries. Consumers do not assemble the package
or install native libraries separately. The example lives in `examples/unity`.

Maintainers publish a successful native CI run using the manual **Publish Unity
Package Manager branches** workflow, supplying its run ID. It validates and
publishes both complete packages with ordinary fast-forward pushes. Builds alone
do not change the distribution branches. The first publication is required
before the Git dependencies can resolve; publication requires push authorization.

The instructions below are for package development and CI, not example users.

Runtime source is in `com.icosa.fidget`. The runnable project's
`examples/unity/Assets/FidgetExamples` folder is the single source for UPM samples;
packaging copies it to `Samples~/Examples`, preserving its asset GUIDs. Native
binaries and generated package files are assembled under `target/`, not committed.
Rust/CI build artifacts must contain `ABI: 9` in `BUILD-INFO.txt`; older archives
intentionally fail this check.

For a local Windows package:

```text
python fidget-native/build.py --target x86_64-pc-windows-msvc --smoke
python unity/package_unity.py --artifacts target/native-artifacts --targets x86_64-pc-windows-msvc --output target/unity-package/com.icosa.fidget
```

The output must be a new directory, so old platform binaries cannot silently
survive a rebuild. `--artifacts` accepts either the local native-artifacts layout
or the directories produced by downloading the native CI artifacts. Supply the
required Rust targets after `--targets`; selecting both iOS variants is rejected.
The script writes deterministic asset GUIDs and explicit plugin compatibility
settings. No temporary local Cargo or Unity dependency references are committed.

The native workflow also assembles two installable tarballs after every target
passes: one for all supported desktop/mobile device architectures, and one for
macOS plus the ARM64 iOS simulator. These contain the platform libraries, importer
settings, runtime sources, preview, Editor integration check and license.

See the [package README](com.icosa.fidget/README.md) for the ownership model,
installation, usage and the current validation limits.

Apple Silicon macOS and iOS libraries are cross-compiled on Intel macOS runners
because hosted ARM runner shortages can prevent a complete package build. Before
publishing, run the C smoke test and Unity Editor integration checks on an Apple
Silicon host using the actual CI-built macOS ARM64 library. The CI build record
correctly marks that target's desktop smoke execution as false; local runtime
validation is an additional release check.

Push and PR runs for the same source head share a concurrency group, retaining
both checks while preventing them from competing for macOS runners.
