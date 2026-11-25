# Modelling with ShapeBuilder

`ShapeBuilder` and `ShapeBuilder<T>` provide the shape, join, repetition and
coordinate operations adapted from Mercury hg_sdf and Inigo Quilez's distance
functions. Package 0.7.0 consolidates these into the existing builder API;
`HgSdf` and `IqSdf` are no longer public APIs.

```csharp
protected override Expression Define(ShapeBuilder<MyShape> shape)
{
    var radius = shape.Parameter(mesh => mesh.Radius);
    var box = shape.RoundedBox(new Expression3(0.6f, 0.8f, 0.4f), radius);
    return shape.TwistY(box, shape.Parameter(mesh => mesh.Twist));
}
```

All methods return the same `Expression` type and compose with existing shapes.
A modelling operation takes its input expression first. It constructs a graph
once; expression-valued dimensions and counts can update without rebuilding it.
Use `shape.Position`, or `shape.X`, `shape.Y`, `shape.Z`, for custom formulas.
`Remap(expression, coordinates)` substitutes coordinate expressions without
requiring a callback or mutable coordinate reference.

## Runnable examples

Import the Examples sample and browse `Scenes/` by capability: Basics,
Parameters, Sharing, Solids, Profiles, Construction, Combinations, Transforms,
and Repetition. Feature scenes such as `Solids/Cones`, `Profiles/Rectangles`,
`Combinations/RoundedJoins` and `Repetition/Polar` compare a few related recipes.
Each displayed recipe has its own `FidgetMesh<T>` and sampling bounds.
Parameterized recipes expose their shape controls in the Inspector. Placement uses GameObject transforms; unrelated display shapes never
share a union or sampling volume. Profiles are extruded into thin solids.

The scripts use the same folder structure as the scenes. Select a shape in Play
mode and edit its property to update the mesh without rebuilding its expression.

## Coordinates and dimensions

- Profiles live in XY. Axial solids point along Y; profiles revolved with
  `RevolveY` use XY as radial/Y coordinates. Surface methods document their plane.
- Box and rectangle dimensions are half extents. Axial solids use `halfHeight`;
  capsules use `halfLength` between their sphere centers. Endpoint overloads use
  the supplied endpoints. `Plane(normal, offset)` places its plane at
  `dot(normalizedNormal, position) = offset`.
- Angles are radians. Aperture parameters are half angles. Twist and bend
  strengths are radians per model unit.
- `Torus` and `CappedTorus` take major radius before minor radius.
- `Subtract(shape, cutout)` and every subtraction variant remove the second
  operand from the first. This reverses IQ's original subtraction argument order.
- `Round(expression, radius)` expands an isosurface. `RoundedBox`,
  `RoundedRectangle`, `RoundedCylinder`, and `RoundedX` preserve their supplied
  outer extents instead. `RoundedCone` describes the hull of endpoint spheres,
  whose radii and center separation are explicit.
- Unit-sized decorative profiles such as `Heart`, `RoundedCross`, `CoolS` and
  `QuadraticCircle` retain their source shape parameters. Use
  `ScaleUniformExact` to give those profiles a uniform model scale.

## Field values and exactness

The existing `Box`, `Rectangle`, `Cylinder`, `ExtrudeZ` and scaling methods retain
their behavior. Their simpler fields are useful for meshing, but do not measure
Euclidean distance at every point. `BoxExact`, `RectangleExact`, `CylinderExact`
and `ExtrudeZExact` provide distance-aware alternatives. The latter requires an
exact XY input profile. Endpoint `Cylinder(start, end, radius)` is a new exact
constructor; the existing axial `Cylinder(radius, halfHeight)` is unchanged.

New constructors use exact formulas where available. Sphere, circle and torus
already did, so no duplicate `SphereExact` or `TorusExact` methods are added.
`Bound` names mark approximate distance fields, including `EllipsoidBound`,
`CrossBound`, `BlobBound` and the plane/power-norm polyhedra. Power-norm fields
are not universally safe Euclidean raymarching bounds for arbitrary exponents.

Exactness is a property of a formula under its documented assumptions, not a
property enforced by the `Expression` type. CSG, smooth joins, scaling and domain
folding can weaken distance guarantees. Subtracting a radius from the existing
cheap box expands a sharp box; use `Round(BoxExact(...), radius)` for rounded
corners. `Shell` produces uniform geometric thickness only with an exact input.

`ScaleUniform` retains the existing coordinate-only behavior.
`ScaleUniformExact(expression, scale)` also multiplies the field by `abs(scale)`;
nonzero negative scales reflect as well as scale. Nonuniform scaling is not exact.
Twist, bend and displacement generally distort distances.

## Solids and surfaces

| Family | Builder methods |
| --- | --- |
| Basic | `Sphere`, `Box`, `BoxExact`, `RoundedBox`, `BoxFrame`, `Plane` |
| Cylinders | `Cylinder`, `CylinderExact`, `InfiniteCylinder`, `RoundedCylinder` |
| Cones/capsules | `Cone`, `Frustum`, `InfiniteCone`, `Capsule`, `RoundedCone` |
| Rings | `Torus`, `CappedTorus`, `Link` |
| Prisms | `HexagonalPrism`, `HexagonalPrismBound`, `TriangularPrism`, `TriangularPrismBound`, `RhombusPrism` |
| Spherical forms | `SolidAngle`, `CutSphere`, `CutHollowSphere`, `SphereCavity`, `VesicaSegment` |
| Polyhedra | `Octahedron`, `OctahedronBound`, `OctahedronPowerBound`, `Pyramid`, `DodecahedronBound`, `IcosahedronBound`, `TruncatedOctahedronBound`, `TruncatedIcosahedronBound`, `PolyhedronBound` |
| Other bounds | `EllipsoidBound`, `BlobBound` |
| Open 3D surfaces | `TriangleSurface`, `QuadSurface`, `SegmentSurface`, `CircleCurve`, `DiscSurface` |

`TriangleSurface` and `QuadSurface` return unsigned distances. They describe open
surfaces, not enclosed solids. Use `Shell(surface, halfThickness)` before meshing.
`CircleCurve` and `DiscSurface` live in XY. `TriangleSurface` requires noncollinear
vertices; a quad must be planar, convex and ordered around its perimeter.

The generalized polyhedra use Mercury's direction table, inclusive indices 0..18.
Their radius is the plane offset, whereas `Octahedron`/`OctahedronBound` use the
axial vertex extent. Exponent overloads retain the power-norm variants.
Hexagonal prism radii are inradii; triangular prism radii are circumradii.
`Pyramid` places its base at `-halfHeight` and its tip at `+halfHeight`.

## XY profiles and curves

| Family | Builder methods |
| --- | --- |
| Rectangles | `Rectangle`, `RectangleExact`, `RoundedRectangle`, `ChamferedRectangle`, `OrientedRectangle` |
| Polygons | `Triangle`, `EquilateralTriangle`, `IsoscelesTriangle`, `Rhombus`, `Trapezoid`, `Parallelogram`, `Pentagon`, `Hexagon`, `Octagon`, `Polygon` |
| Stars | `Star`, `Pentagram`, `Hexagram` |
| Circular forms | `Circle`, `Pie`, `CutCircle`, `Arc`, `RingSector`, `Horseshoe`, `Vesica`, `Moon`, `CircleWave` |
| Decorative | `UnevenCapsule`, `RoundedCross`, `Egg`, `Heart`, `CrossBound`, `RoundedX`, `BlobbyCross`, `Tunnel`, `Stairs`, `CoolS`, `Corner` |
| Curves | `Segment`, `Bezier`, `Ellipse`, `Parabola`, `ParabolaSegment`, `Hyperbola`, `QuadraticCircle` |

`Segment`, `Bezier` and `CircleWave` return unsigned distances; `Round` gives them
a stroke width. Extrude the resulting profile to make a solid.
`RoundedRectangle` supports either one radius or four corner radii in
+X/+Y, +X/-Y, -X/+Y, -X/-Y order. Regular polygon radii are inradii;
`EquilateralTriangle` instead takes base half width, stated by its parameter name.
`Polygon`'s vertex count fixes graph structure, but coordinates can be parameters.

```csharp
var star = shape.Star(radius, shape.Parameter(mesh => mesh.Points), 3);
return shape.ExtrudeZExact(star, -0.2f, 0.2f);
```

## Combining and modifying fields

| Operation | Builder methods |
| --- | --- |
| CSG | `Union`, `Intersect`, `Subtract`, `Xor`; existing `Intersection`, `Difference` aliases remain |
| Quadratic smoothing | `Blend`, `IntersectBlend`, `SubtractBlend` |
| Corner joins | `UnionRound`, `IntersectRound`, `SubtractRound`, `UnionChamfer`, `IntersectChamfer`, `SubtractChamfer` |
| Patterned joins | `UnionColumns`, `IntersectColumns`, `SubtractColumns`, `UnionStairs`, `IntersectStairs`, `SubtractStairs` |
| Features | `Pipe`, `Engrave`, `Groove`, `Tongue` |
| Surface operations | `Round`, `Shell`, `Inverse`, `Displace` |
| Construction | `ExtrudeZ`, `ExtrudeZExact`, `RevolveY`, `LoftZ`, `Elongate`, `ElongateInterior` |
| Placement | Existing `Translate`, `Move`, `Rotate`, `RotateX/Y/Z`, `Reflect`, `Transform`, `Scale`; new `ScaleUniformExact` |
| Deformation | `TwistY`, `BendZ` |

`Blend` keeps its existing radius convention. IQ's original smooth-union amount
maps to `Blend(a, b, 4 * amount)`. Mercury's soft union maps directly to `Blend`.
Nonpositive constant blend radii retain the existing hard-operation fallback.
Mercury's geometric joins assume approximately perpendicular input gradients for
their distance guarantees. Columns require count > 1; stair joins require count > 0.
`ElongateInterior` adds the source's interior correction and expects a centered
primitive symmetric about its coordinate planes. Neither elongation form makes
an arbitrary input field exact.

## Repetition, symmetry and identifiers

`Repeat`, `RepeatXY`, `RepeatLimited`, `RepeatMirrorX`, `RepeatMirrorXY`,
`RepeatGridXY`, `RepeatPositiveX`, and `RepeatIntervalX` take full cell spacing.
The existing `RepeatX` uses a **half-spacing radius**; `RepeatY` and `RepeatZ`
follow that existing convention. `RepeatPolarY` repeats in XZ around Y;
`RepeatPolarZ` repeats XY profiles around Z. Rotation/remapping can orient the
specialized XY or X methods into other planes/axes. In the `Repetition/Polar`
sample, a single angled vent slot is the cutter. Repeating it before subtraction
cuts a ring of slots spaced `360° / Count` apart. Set Count to 1–16 in the
Inspector. Slot length, half width, start radius, bore radius and disc thickness
can be adjusted independently.

`RepeatLimited` takes per-axis spacing and a per-axis limit: cells run from
`-limit` to `+limit`, inclusive. `RepeatIntervalX` accepts explicit first and last
cell indices. Counts, limits, spacing and offsets can be parameters. Use integer
counts and limits where they represent numbers of instances; fractional values
can have seams or incomplete patterns. Spacing must remain positive.

Every repetition has an overload returning its cell identifier via `out`:

```csharp
var part = shape.Translate(shape.Capsule(0.12f, 0.3f), new Vector3(0.65f, 0, 0));
var repeated = shape.RepeatPolarY(part, count, out var sector);
return shape.Round(repeated, sector.Modulo(2) * 0.02f);
```

Identifiers are expressions evaluated in the modifier's input coordinate space,
not C# integers. Use expression arithmetic or `Select` to vary copies. Combine
identifier-dependent fields before applying further transforms to the result.
Simple domain repetition assumes shapes match at cell boundaries; it does not
search neighboring cells for arbitrary asymmetric or oversized shapes.

`SymmetryX/Y/Z/XZ` copies positive coordinate sides across the origin.
`MirrorX/Y/Z` also subtracts an offset and can return the original side sign.
`MirrorOctantXY` additionally swaps coordinates into one octant. `FoldPlane`
reflects only one side of a plane, whereas existing `Reflect` reflects the entire
object. Plane-fold normals may be fixed Unity vectors or expression vectors.

## Expression math

Unity `Vector2/3/4` convert implicitly to `Expression2/3/4`.
These support component arithmetic, swizzles, `Abs`, `Floor`, `SignNonZero`,
`Min`, `Max`, `MinComponent`, `MaxComponent`, `Dot`, `LengthSquared`, `Length`,
and `LengthPower`; 3D vectors also provide `Cross`. Scalars provide `Square`,
`Clamp`, `Saturate` and `SignNonZero` alongside the existing expression math.
`SignNonZero(0)` is +1, preserving Mercury's helper convention;
`Compare(0)` is the three-way sign whose value at zero is 0.
`LengthPower(6)` and `LengthPower(8)` cover IQ's alternative metrics.

Use positive dimensions and valid curve parameters. Rounded-cone center separation
must exceed the radius difference. Polygon edges, segment lengths and plane
normals must be nonzero. Corner radii must fit within the supplied extents.
Expression-valued constraints must remain valid after parameter updates; graph
construction cannot validate all future parameter values.

The rhombus formulas use a nonzero sign at their axial tips, avoiding the source's
spurious zero-distance rays beyond those tips. `Vesica` swaps its coordinate
orientation when height exceeds width. Endpoint cylinders and spherical sectors
use equivalent distance constructions that retain cap/apex derivatives.
Surface-normal preparation can sample the full field when a selected zero-sign
trace is locally constant at a valid face, and orients corner derivatives into
the same hemisphere as that face-interior derivative.

The source blob and ellipsoid bound retain singularities at the origin. Closed-form
curve solvers can be ill-conditioned near repeated roots and symmetry axes.
Meshing still needs explicit bounds, adequate depth, finite samples and meaningful
surface gradients. Full Mercury formula comparisons, IQ independent distance
checks, builder convention checks and representative native meshes are in
`unity/tests`; Editor probes verify the worker and sample paths. These checks
are not player/device performance measurements.

Formula sources and notices: [Mercury migration](HgSdf.md),
[IQ migration](IqSdf.md), [third-party notices](ThirdPartyNotices.md).
