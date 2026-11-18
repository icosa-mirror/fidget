# Fidget examples

Import Examples in Package Manager, open a scene under `Scenes/`, and press Play.
Requires Fidget 0.7.0 or later. The sample assemblies are skipped for older packages.

Start with `Basics/Primitives` or `Combinations/AnimatedSubtraction`.
`Basics/AnimatedSphere` shows a minimal animated property, `Parameters/InteractiveParameters`
adds on-screen torus controls, and `Sharing/SharedDefinitions` shares one expression
between independently parameterized objects.

Scenes and scripts follow the same capability folders:

- **Solids**: capsules, cones, rings, prisms, polyhedra, rounded solids and thickened surfaces.
- **Profiles**: rectangles, triangles, polygons, stars, lenses, curves and decorative profiles.
- **Construction**: extrusion, revolution, lofting and blended extrusion.
- **Combinations**: smooth, rounded and chamfered joins, columns, stairs and surface features.
- **Transforms**: twist, bend, displacement, elongation, rounding and shells.
- **Repetition**: axes, grids, finite intervals, mirrors, symmetry, folding and polar copies.

Each feature scene compares a few related recipes. Each displayed recipe has its
own `FidgetMesh<T>`, fitted sampling bounds and Inspector control. GameObject
transforms arrange the display; unrelated shapes never share a display union.
The repetition and CSG recipes combine shapes when demonstrating those operations.
Profiles use thin extrusions so they can be meshed.

Select a shape during Play and change its Inspector property. `Define` registers
that property with `shape.Parameter`, so the mesh updates without rebuilding its
expression. Change Detail to adjust resolution. Bounds cover the demonstrated
parameter range; adjust Bounds and rebuild when extending a recipe.

All modelling methods belong to `ShapeBuilder`. See the package's `SdfModelling.md`
for conventions and distance guarantees. Source attributions and Mercury's MIT
notice are included in `ThirdPartyNotices.md`.

The Editor tools create scenes through Unity's scene API and fit each mesh's
bounds across its parameter range. Close any feature scene you intend to recreate,
then use **Fidget > Create Feature Examples**. The four introductory scenes are
maintained separately. To validate a feature scene through Unity CLI eval:

```csharp
return FidgetExamples.Editor.ExampleValidation.Run("Solids/Cones");
```

This checks nonempty meshes, finite vertices and surface normals, updates without
graph rebuilds, and bounds margins at five parameter settings. Rendered previews
are written beneath `target/example-checks` when run in the source repository.
