# Fidget for Unity 6.3

This integration uses the CPU mesher on a Unity worker job and hands completed
buffers to a persistent Unity `Mesh`. The sphere is the initial geometry source;
the transfer path does not assume constant vertex counts or topology.

## Data path and ownership

1. `Schedule` allocates one writable `MeshData` entry and schedules an `IJob`.
2. The job calls Rust to generate a mesh. The native handle owns Fidget's original
   vertex and triangle arrays; there are no additional packed staging arrays.
3. The job queries counts and sizes Unity's writable vertex/index buffers. Rust
   writes directly to their pointers, packing positions and converting indices
   to 32-bit in one pass per array. It computes bounds in the position pass.
4. The job frees the original native mesh. Unity's populated buffers now contain
   the only application-owned copy of the completed geometry.
5. `TryApply` checks completion, then applies and disposes the writable data on
   the main thread. The same `Mesh` object is reused, with explicit new bounds.

There are no managed vertex/index arrays, per-update Tasks, pinning, normal
recalculation, main-thread bounds scans, or queues of stale shape requests.
Layout and result storage are persistent native arrays. Only one job may be in
flight per stream; busy `Schedule` calls return false. Submit current parameters
again after applying the finished result. The last completed mesh stays visible.

Fidget still allocates its own mesh, and Unity allocates writable buffers for
each update. Unity controls the final GPU upload and graphics-buffer allocation;
this is not a claim of zero GPU copies or zero engine allocations. Changing
topology is handled without guessing capacities or retaining worst-case buffers
on mobile. The same path handles unchanged counts without special cases.

`Mesh.GetNativeVertexBufferPtr` gives a graphics-API resource handle, not a CPU
pointer that Rust can write through. Direct Metal/Vulkan/D3D buffer integration
would need resource ownership, synchronization and render-thread handling for
each backend. This package uses the portable
[writable MeshData API](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/mesh/allocatewritablemeshdata)
and leaves those GPU responsibilities with Unity.

## Install and display

Use an assembled package containing ABI-v2 libraries, not this source directory
alone. Phase-1 ABI-v1 binaries are rejected before any worker job is scheduled.
Install the generated `.tgz` through Package Manager's **Install package from
tarball**, or add the assembled folder's `package.json` as a local package.

1. Add a `ChangingSphere` component to a GameObject. It supplies MeshFilter and
   MeshRenderer automatically.
2. Create a material using the included `Fidget/Preview` shader and assign it to
   the renderer. Put a camera in front of the object and enter Play mode.
3. Adjust radius, amplitude, speed and detail in the Inspector. Each completed
   update replaces the displayed geometry. Busy frames keep the previous mesh.

The preview shader derives flat lighting from fragment derivatives, so it needs
only positions and indices. General production materials will need an explicit
normal/UV strategy; the adapter does not silently add these costs.

For application code, construct `SphereMeshStream` on the main thread, assign its
`Mesh` to your renderer, call `TryApply` followed by `Schedule`, and dispose it
on the main thread at teardown. `TryApply` does not wait for an unfinished job.
`Dispose` must wait for any running native call before releasing its buffers.
Do not dispose the exposed Mesh separately or call the controller concurrently.

## Platform details

Plugin import settings select the appropriate OS and CPU, including the Editor.
iOS calls use `__Internal`; other targets use the platform shared library.
Choose the iOS **device** package for device builds. Simulator archives are
packaged separately because both variants are ARM64 and must never be linked
together. The iOS build hook adds Rust's `iconv` link dependency to UnityFramework.

The integration uses a non-Burst Unity job: the Rust library performs the native
computation. Fidget's internal threading is disabled for this probe, avoiding a
second worker pool per generation request. There are no managed callbacks from
Rust, and all P/Invoke arguments are fixed-layout values or pointers.

## Validation

`Icosa.Fidget.Editor.IntegrationCheck.Run` is an Editor batch entry point for a
dedicated validation project. It runs two job updates with different topology,
checks resulting bounds/indices/positions, renders the mesh to a texture, and
disposes a stream with work still in flight. It logs `[FidgetUnity] PASS` and
saves `Logs/fidget-preview.png`. This is an integration check, not a benchmark.

Windows Unity 6000.3.18f1 has passed that check with graphics enabled. The local
Package Manager failed before compilation, so this execution used the assembled
package under `Assets/Fidget` with `-noUpm`. UPM installation, IL2CPP players and
mobile Unity runtime execution still need validation; native CI builds alone do
not establish those results.
