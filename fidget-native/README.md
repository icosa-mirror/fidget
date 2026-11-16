# Native meshing adapter

This crate exposes Fidget's VM evaluator and manifold dual-contouring mesher
through a C ABI. The Unity integration uses reusable meshers on worker jobs;
there is no JIT or managed callback. The upstream `workspace-hack` dependency
unifies development dependencies (including WGPU on some desktop targets),
but those are not used to generate these meshes.

## C API

ABI **v6** is defined in [include/fidget_native.h](include/fidget_native.h).
Applications should normally use the [Unity API](../unity/com.icosa.fidget/README.md).

1. `fidget_shape_compile` compiles a topologically ordered expression graph with
   parameter slots. The final node is the root. Arithmetic, Boolean operations,
   and coordinate remapping are supported; the header defines each opcode.
2. `fidget_mesher_create` retains the compiled shape and creates reusable evaluator
   state. The original shape handle may then be freed. Independent meshers can
   run concurrently; each individual mesher is single-caller.
3. `fidget_mesher_generate` takes parameter values, explicit bounds, and depth
   (1 through 8). It produces a new native mesh synchronously without spawning
   internal worker threads.
4. Optionally call `fidget_mesh_prepare_surface_normals` before querying counts.
   It selects a branch trace at each triangle centroid and evaluates that
   branch's gradients at the corners. Singular corners use the face-interior
   gradient, falling back to numerical derivatives if necessary. Differing normals split render vertices
   without moving geometry; no crease-angle threshold is used. A triangle
   spanning a branch boundary cannot represent that transition exactly.
   Preparation must use the mesher's most recent generated mesh and parameters.
   If preparation fails, free the mesh: its input has been consumed.
5. `fidget_mesh_get_counts` reports output sizes. `fidget_mesh_copy_to` writes
   positions and 32-bit indices directly into caller-owned buffers and computes
   bounds during the position copy. `fidget_mesh_write_normals` copies prepared
   surface normals, or computes area-weighted smooth normals if unprepared.
6. `fidget_mesh_free` releases the native mesh after copying. Caller buffers
   remain valid. Free the mesher when its stream is finished.

The original `fidget_mesh_sphere` entry point remains as a simple probe with a
fixed `[-1, 1]^3` domain. Radius must be finite and strictly between zero and one.

Rebuild native libraries when the ABI changes; Unity checks v6 before scheduling
work. Never free handles while another thread is using them. Rust panics become
status codes; invalid pointers and process aborts such as out-of-memory cannot
be recovered. Mesh output has no UVs or materials. Thin features below the
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
errors, parameter updates, shared shape lifetimes, explicit bounds, finite
positions, index bounds, and both normal modes. It checks ABI layout through the public header, rather than calling Rust
functions directly from a Rust test.

## Scope

Phase 1 establishes compilation, linking and artifact packaging. Desktop C
smoke tests establish basic native execution. Mobile builds alone do not prove
device execution, Unity/IL2CPP integration, distribution packaging or frame
budget. Per-frame geometry generation still requires representative CPU meshing,
allocation and upload measurements on the intended devices.
