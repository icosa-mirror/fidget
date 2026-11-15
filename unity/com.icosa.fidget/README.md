# Fidget for Unity

Create implicit geometry in C#, compile its expression once, and update parameters
without rebuilding the expression. Native libraries are included in the UPM
package for Windows x64, macOS Intel/ARM64, Linux x64, Android ARM64/x64, and iOS
ARM64. The evaluator uses Fidget's VM, not runtime-generated machine code.

## Install

In Package Manager, choose **Install package from Git URL** and enter:

```text
https://github.com/icosa-mirror/fidget.git#upm
```

For reproducible projects, pin the published commit instead of the moving branch.
Use the `upm-simulator` distribution for an ARM64 iOS simulator; do not install
both iOS variants. No separate native-library download or Rust installation is
needed. The package source directory alone does not contain the native binaries.

The committed example project uses Unity 6000.6.0f1 and URP 17.6.0. The runtime
package requires Unity 6.3 or later. Import **Examples** from this package's Samples
tab for the scripts and scenes described below.

## Component workflow

Add `FidgetMesh` to a GameObject and assign a material to its MeshRenderer. Build a
shape in your script's `Start`, then update its parameters in `Update`:

```csharp
public sealed class Cutter : FidgetMesh<Cutter>
{
    public float Offset { get; set; }
    public float Radius { get; set; } = 0.32f;

    protected override Expression Define(ShapeBuilder<Cutter> s)
    {
        var body = s.Intersect(s.Sphere(0.85f), s.Box(Vector3.one * 0.65f));
        var hole = s.Translate(s.Cylinder(s.Parameter(c => c.Radius), 1.2f),
                               s.Parameter(c => c.Offset), 0, 0);
        return s.Subtract(body, hole);
    }
}
```

Attach `Cutter` to a GameObject. Other scripts update it directly:

```csharp
cutter.Offset = Mathf.Sin(Time.time) * 0.4f;
cutter.Radius = 0.3f;
```

The component calls `Define` once on first enable, prepares the expression, and
owns the native resources and Unity mesh. `Detail` defaults to 6; `Bounds` and
`Normals` are Inspector settings. The default normal mode is `Surface`. Add
`[field: SerializeField]` to a property to expose its backing field in the Inspector,
as shown in the example components.

New components default to updating when property values change. Enable `Continuous`
to generate every available frame. Updates happen in `LateUpdate`; values are
snapshotted before the worker runs. Disabling completes outstanding work and
releases mesh buffers; reenabling reuses the prepared expression. Destruction
releases it. Call `Rebuild()` after changing expression structure, bounds, or normal
mode; ordinary property changes require no rebuild.

Property accessors are ordinary C# delegates, compatible with IL2CPP: no reflection,
runtime code generation, or string lookup. Keep them inexpensive and free of side
effects. Binding storage is reused across frames. If you override `OnEnable`,
`OnDisable`, `LateUpdate`, `OnDestroy`, or `Reset`, call the base implementation.

## Direct mesh-stream workflow

Compile one typed definition and bind independent model instances to its streams:

```csharp
// A plain C# model for this advanced sharing workflow:
// public sealed class SphereValues { public float Radius { get; set; } = 0.6f; }
var builder = new ShapeBuilder<SphereValues>();
var shape = builder.Compile(builder.Sphere(builder.Parameter(s => s.Radius)));
var mesh = new SphereValues();
var parameters = shape.CreateParameters(mesh);
var stream = new MeshStream(shape.Shape, settings);
meshFilter.sharedMesh = stream.Mesh;

// Each frame:
mesh.Radius = radius;
stream.TryApply();
stream.TrySchedule(parameters);

// During teardown, on the main thread:
stream.Dispose();
shape.Dispose();
```

Each call to `CreateParameters(model)` binds that model independently, so another
stream can use a different `SphereValues` without recompiling the expression. The
low-level non-generic builder and explicit parameter handles remain available for
dynamically constructed graphs; named parameters are optional there.

Keep the compiled shape until all desired streams have been created. Existing
streams retain the native expression and remain usable after `shape.Dispose()`.
Do not destroy `stream.Mesh` separately. All public controller and parameter APIs
are main-thread APIs; do not call them concurrently.

`TrySchedule` returns false while busy, without queuing the request. On success it
copies parameter values into a persistent native snapshot, so subsequent parameter
edits cannot change work already running. The previous completed mesh remains
visible. `TryApply` never waits for unfinished work; disposal does wait, to avoid
freeing resources a job still uses. An overload accepts a per-request depth.

## Shapes and settings

1. `Sphere`, `Box` (half extents), `Cylinder` (Y axis), and `Torus` produce implicit
   expressions. `Union`, `Intersect`, and `Subtract` combine them.
2. Arithmetic, `Min`, `Max`, `Abs`, `Sqrt`, `Square`, `Sin`, and `Cos` support custom
   expressions using the builder's `X`, `Y`, and `Z` coordinates.
3. `Translate`, `RotateY` (radians), affine `Transform`, and general `Remap` change
   coordinates. Expressions and low-level parameter keys must come from the same builder.
4. `MeshSettings.Bounds` is an explicit local-space sampling domain. Keep the whole
   surface inside it. Geometry outside the domain is not generated. Depth ranges
   from 1 to 8; thin details below the sampling resolution can be missed.
5. `MeshNormals.None` writes positions and indices only. `MeshNormals.Smooth`
   additionally writes area-weighted smooth normals for ordinary lit materials.
   This costs a worker-side triangle pass; it does not split hard edges or add UVs.
   Use solid-color URP Lit or triplanar materials when UVs are unnecessary.
6. `MeshNormals.Surface` evaluates Fidget's expression gradients using the branch
   trace selected at each triangle's centroid. Different corner normals get
   separate render vertices; geometric positions and triangles are unchanged.
   There is no crease-angle threshold. This is recovered face classification,
   not provenance exported by the mesher: a triangle spanning a branch boundary
   cannot represent that transition exactly. It adds expression evaluation,
   normal storage, and potentially more render vertices. The rounded-box Boolean
   example has been visually reviewed; this is not a universal hard-edge guarantee.

Parameter values must be finite; applications must also keep expressions valid
(e.g. avoid division by zero and negative square roots). Bounds, depth, graph
references, and parameter ownership are checked before native work where possible.
Invalid arguments and native failures produce descriptive managed exceptions.

## Examples

1. **Animated Boolean**: a cylinder cuts a rounded box. Its component derives from
   `FidgetMesh<AnimatedBoolean>` and defines the expression in `Define`.
2. **Interactive Parameters**: change a torus through Inspector controls or bind
   UI sliders to its setters. Regenerates only when parameters change.
3. **Shared Shape Instances**: one compiled shape, independent parameters and
   jobs for multiple objects. Shows explicit lifetime management with `MeshStream`.

`ChangingSphere` lives with the imported example scripts. It derives from
`FidgetMesh<ChangingSphere>`, defines a sphere, and animates its `Radius` property.
`SphereMeshStream` remains available as a lower-level runtime API.

## Data path

The worker calls Fidget, sizes Unity writable MeshData buffers to the actual output,
and writes positions/indices directly into those buffers. Bounds are computed in
the position-copy pass. Optional normals use a separate vertex stream. The worker
then frees Fidget's original mesh. The main thread applies the result to one
persistent Unity Mesh without rescanning bounds or indices.

There are no managed vertex/index arrays, managed callbacks from Rust, per-frame
Tasks, or an additional native thread pool. The compiled expression and parameter
storage are reused. Fidget still allocates meshing scratch/output, and Unity
allocates writable buffers and performs the GPU upload. This is not a zero-copy
GPU implementation. Direct graphics-buffer handles are not CPU-writable pointers.

## Validation and platforms

The native C smoke test covers parameter updates, shared-shape lifetime, custom
bounds, normals, output indices, and invalid arguments. Native CI builds every
packaged target and checks Android 16 KB alignment and iOS static linking.

Native build results do not establish Unity player or device compatibility.
Record Windows Editor, IL2CPP player, and mobile-device checks separately when
releasing a new package. The iOS post-build hook links iconv into UnityFramework.
The example project's README records its current live validation results.
