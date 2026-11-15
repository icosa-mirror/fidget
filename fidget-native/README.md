# Native meshing build probe

This crate establishes a C ABI and platform builds for a future Unity 6.3
integration. It uses `fidget-core`'s VM evaluator and `fidget-mesh`, without
`fidget-jit`. It is not a production per-frame meshing API or a Unity package.
The upstream `workspace-hack` dependency still unifies development dependencies
(including WGPU on some desktop targets); it is not used to generate this mesh.

## C API

ABI v2 is defined in [include/fidget_native.h](include/fidget_native.h).
`fidget_mesh_sphere(0.6f, 4, &mesh)` generates a sphere in `[-1, 1]^3`.
`fidget_mesh_get_counts` reports output sizes, then `fidget_mesh_copy_to` writes
positions and 32-bit indices into caller-owned buffers. Bounds are calculated
in the same position-copy pass. `fidget_mesh_free` releases the original mesh;
the caller's copied buffers remain valid. The native handle retains Fidget's
original mesh, without first allocating separate packed arrays.

This deliberately replaces the phase-1 borrowed-view ABI. Rebuild all native
libraries when upgrading; Unity checks for ABI v2 before scheduling work.

The probe is synchronous and single-threaded, and allocates a fresh mesh per
call. Independent calls may run concurrently. Do not free a handle while
another thread copies its data. Radius must be finite and strictly between
zero and one; depth is limited to 1 through 8. Rust panics become a status code;
invalid pointers and process aborts such as out-of-memory cannot be recovered.

The output has no normals, UVs or materials. Coordinate conventions and winding
conversion for Unity belong to the later integration. Thin features below the
sampling resolution may be missed; Fidget also permits self-intersections.

## Builds

Run these commands from the repository root. Install Rust 1.95.0 (the CI-pinned
version), Python 3.10 or newer, and the target's native linker/SDK first. Desktop
smoke tests also require CMake 3.20 or newer and a C compiler.

| OS / architecture | Rust target | Artifact | Build host / verification |
|---|---|---|---|
| Windows x86-64 | `x86_64-pc-windows-msvc` | `fidget_native.dll` + import library | Windows + MSVC; C smoke execution |
| macOS Apple Silicon | `aarch64-apple-darwin` | `libfidget_native.dylib` | Apple Silicon Mac + Xcode; C smoke execution |
| macOS Intel | `x86_64-apple-darwin` | `libfidget_native.dylib` | Intel Mac + Xcode; C smoke execution |
| Linux x86-64 | `x86_64-unknown-linux-gnu` | `libfidget_native.so` | Ubuntu 22.04 + C compiler; C smoke execution |
| iOS ARM64 device | `aarch64-apple-ios` | `libfidget_native.a` | Mac + Xcode iPhoneOS SDK; C link probe |
| iOS ARM64 simulator | `aarch64-apple-ios-sim` | `libfidget_native.a` | Mac + Xcode simulator SDK; C link probe |
| Android ARM64 | `aarch64-linux-android` | `libfidget_native.so` | NDK r28+; ELF and export checks |
| Android x86-64 | `x86_64-linux-android` | `libfidget_native.so` | NDK r28+; ELF and export checks |

For example, on Windows:

```text
rustup target add x86_64-pc-windows-msvc
python fidget-native/build.py --target x86_64-pc-windows-msvc --smoke
```

On macOS/Linux, substitute the matching target and use `python3` if needed.
`--smoke` requires the build host architecture to match the target. For iOS,
install both targets and build each separately without `--smoke`; the script
links a C executable against each static archive but does not run it.

For Android, install the targets and provide the NDK directory:

```text
rustup target add aarch64-linux-android x86_64-linux-android
python fidget-native/build.py --target aarch64-linux-android --android-ndk <NDK-directory>
python fidget-native/build.py --target x86_64-linux-android --android-ndk <NDK-directory>
```

`ANDROID_NDK_HOME` or `ANDROID_NDK_ROOT` can replace `--android-ndk`. The script
uses Android API 23 and explicitly requests 16 KB LOAD/RELRO linker alignment;
it checks LOAD alignment, file/virtual offset congruence and C exports in the
finished ELF. CI pins NDK r28b (`28.1.13356709`). See Android's
[16 KB page-size guidance](https://developer.android.com/guide/practices/page-sizes).
APK/AAB ZIP alignment and actual device execution remain app-level checks.

Apple builds default to macOS 11.0 and iOS 13.0 native deployment targets;
`MACOSX_DEPLOYMENT_TARGET` and `IPHONEOS_DEPLOYMENT_TARGET` can override them.
The Unity app's minimum OS requirements still apply. iOS builds record rustc's
native static link requirements in `BUILD-INFO.txt`, and verify them by linking
the C probe with the appropriate Xcode SDK. See
[Rust's iOS target documentation](https://doc.rust-lang.org/rustc/platform-support/apple-ios.html).

Outputs are saved under `target/native-artifacts/<Rust-target>/`, alongside the
header, license, this README and build provenance. Desktop and Android builds
produce shared libraries; iOS builds produce only a static archive. Build
intermediates and smoke executables remain under `target/`. The two macOS
architectures are separate artifacts, as are iOS device and simulator archives.

The [native workflow](../.github/workflows/native.yml) builds all eight rows,
executes the four desktop probes, links the two iOS probes and checks the two
Android ELF files. It runs on relevant pushes to `main` and
`feature/unity-native-platform-builds`, relevant PR updates targeting `main`,
and manual dispatch. Each successful job uploads its own artifact.

The C executable exercises the actual shared-library boundary, checks argument
errors, verifies finite sphere positions and index bounds, and releases several
meshes. It checks ABI layout through the public header, rather than calling Rust
functions directly from a Rust test.

## Scope

Phase 1 establishes compilation, linking and artifact packaging. Desktop C
smoke tests establish basic native execution. Mobile builds alone do not prove
device execution, Unity/IL2CPP integration, distribution packaging or frame
budget. Per-frame geometry generation still requires representative CPU meshing,
allocation and upload measurements on the intended devices.
