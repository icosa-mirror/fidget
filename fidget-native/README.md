# Native meshing build probe

This crate establishes a C ABI and platform builds for a future Unity 6.3
integration. It uses `fidget-core`'s VM evaluator and `fidget-mesh`, without
`fidget-jit`. It is not a production per-frame meshing API or a Unity package.
The upstream `workspace-hack` dependency still unifies development dependencies
(including WGPU on some desktop targets); it is not used to generate this mesh.

## C API

See [include/fidget_native.h](include/fidget_native.h) for the ABI and ownership
contract. `fidget_mesh_sphere(0.6f, 4, &mesh)` generates a sphere in `[-1, 1]^3`.
`fidget_mesh_get_view` borrows packed XYZ `float` positions and `uint32_t`
triangle indices. `fidget_mesh_free` releases the handle and both buffers.
The view uses normal platform C struct alignment; do not apply packed layout.

The probe is synchronous and single-threaded, and allocates a fresh mesh per
call. Independent calls may run concurrently. Do not free a handle while
another thread reads its buffers. Radius must be finite and strictly between
zero and one; depth is limited to 1 through 8. Rust panics become a status code;
invalid pointers and process aborts such as out-of-memory cannot be recovered.

The view has no normals, UVs or materials. Coordinate conventions and winding
conversion for Unity belong to the later integration. Thin features below the
sampling resolution may be missed; Fidget also permits self-intersections.

## Local Windows smoke test

From the repository root, with Rust 1.95, CMake and a C compiler installed:

```powershell
cargo build -p fidget-native --release --locked
cmake -S fidget-native/tests -B target/native-smoke/windows `
  -DFIDGET_LIBRARY="$PWD/target/release/fidget_native.dll.lib" `
  -DFIDGET_RUNTIME="$PWD/target/release/fidget_native.dll"
cmake --build target/native-smoke/windows --config Release
ctest --test-dir target/native-smoke/windows -C Release --output-on-failure
```

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
