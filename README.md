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

## Shared component definitions

Create another component using an existing component's prepared expression:

```csharp
var instance = source.CreateSharedInstance(parent);
instance.Radius = 0.7f;
```

This creates a plain shape GameObject, copying the source's meshing settings and
materials. Shape properties start at their class defaults and remain independent.
Each instance owns its own mesh and worker state. The definition is prepared only
once and retained independently, so destroying or rebuilding the source does not
invalidate its existing instances. Instances can create further shared instances.

To share with an existing component of the same type, call
`target.ShareDefinition(source)`. Its property values and settings are preserved.
Sharing selects the source's current expression; subsequent `Rebuild()` calls
change only the rebuilt component. Bind changing inputs through the accessor's
argument (`mesh => mesh.Radius`), so each instance reads its own values.

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

### Standard shape library

The builder also exposes `Circle`, `Rectangle`, centered `Sphere`, corner-based
`Box(lower, upper)`, `Blend`, `Inverse`, `Difference`, multi-input `Union` and
`Intersection`, `Move`, `Scale`, `ScaleUniform`, `Reflect` / `ReflectX` / `ReflectY`
/ `ReflectZ` / `ReflectXY`, arbitrary-axis `Rotate`, `RotateX`, `RotateZ`,
`RevolveY`, `ExtrudeZ`, `LoftZ`, and `RepeatX`. These methods build expressions in
C#; no Rust scripting or separate `fidget-shapes` installation is required.
They work on both `ShapeBuilder` and `ShapeBuilder<T>`.

```csharp
protected override Expression Define(ShapeBuilder<MyShape> s)
{
    var radius = s.Parameter(mesh => mesh.Radius);
    var profile = s.Circle(radius);
    var solid = s.ExtrudeZ(profile, -0.3f, 0.3f);
    return s.Blend(solid, s.Translate(s.Sphere(0.4f), 0.4f, 0, 0), 0.15f);
}
```

Conventions and differences from the Rust structs:

- All rotation methods use **radians**, including the existing `RotateY`.
  Multiply degrees by `Mathf.Deg2Rad`. Rotations accept a fixed axis/center and
  expression-valued angle; scaling, reflection offsets, extrusion/loft limits,
  blend radius, and repetition spacing can also use bound parameters.
- `Box(halfExtents)` remains available; `Box(lower, upper)` uses corners.
  `Circle(center, radius)` uses XY, and `Sphere(center, radius)` uses XYZ.
- `RevolveY` maps an XY profile to `(sqrt(X² + Z²) - offset, Y, 0)`.
  A circle centered at the origin with a positive radial offset creates a torus.
  This uses the geometric Y axis; the current Rust helper's coordinate mapping
  differs. `ExtrudeZ` and `LoftZ` sample their profiles at Z = 0.
- `Reflect` normalizes its supplied normal; offset is measured along that unit
  normal. `ReflectXY` reflects across X = Y at offset zero.
- Multi-input `Union` and `Intersection` require at least one input. Unlike the
  Rust empty-list convention, they do not emit infinite constants.
- Keep scale factors nonzero, blend/repetition radii positive, and loft upper
  greater than lower. Constant non-positive blend radius falls back to union.
  Scale remaps coordinates without correcting field magnitudes, as in Rust.
- `RepeatX` repeats every `2 * radius`, using Euclidean modulo, so negative X
  wraps correctly. Its offset follows the Rust cell-mapping convention. Match
  field values across cell boundaries to avoid discontinuities and use explicit
  meshing bounds to select a finite region.

These APIs require **ABI 5** native libraries. Rebuild/package matching native
libraries before using updated C# sources with an older published package.
The **Shape Library** sample scene uses four small `FidgetMesh<T>` components to show
extrusion, blending, lofting, revolution,
reflection, scaling, rotation, and repetition with editable Inspector properties.

Parameter values must be finite; applications must also keep expressions valid
(e.g. avoid division by zero and negative square roots). Bounds, depth, graph
references, and parameter ownership are checked before native work where possible.
Invalid arguments and native failures produce descriptive managed exceptions.

## Examples

1. **Animated Boolean**: a cylinder cuts a rounded box. Its component derives from
   `FidgetMesh<AnimatedBoolean>` and defines the expression in `Define`.
2. **Interactive Parameters**: change a torus through Inspector controls or bind
   UI sliders to its setters. Regenerates only when parameters change.
3. **Shared Shape Instances**: one shared component definition, independent properties and
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
