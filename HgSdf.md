# Migrating Mercury hg_sdf formulas

Package 0.7.0 exposes the Mercury functionality through `ShapeBuilder` rather
than a separate public `HgSdf` class. Shape operations take expressions first;
coordinate mutation and callbacks are unnecessary for the standard operations.
Copyright (c) 2011-2021 Mercury Demogroup. The full MIT notice remains in
[ThirdPartyNotices.md](ThirdPartyNotices.md).

See [ShapeBuilder modelling](SdfModelling.md) for the complete public API and
its parameter constraints. The original formulas remain internal implementation
details, with reference tests against their source conventions.

| Source function | Builder API |
| --- | --- |
| `Point` | `shape.Position` |
| `saturate` | `expression.Saturate()` |
| `sgn` | `expression.SignNonZero()` |
| `square` | `expression.Square()` or vector multiplication |
| `lengthSqr` | `vector.LengthSquared()` |
| `vmax`, `vmin` | `vector.MaxComponent()` / `MinComponent()` |
| `fSphere` | `Sphere` |
| `fPlane` | `Plane(normal, -sourceOffset)` |
| `fBox`, `fBox2` | `BoxExact` / `RectangleExact` |
| `fBoxCheap`, `fBox2Cheap` | Existing `Box` / `Rectangle` |
| `fCorner`, `fBlob` | `Corner` / `BlobBound` |
| `fCylinder` | Existing `Cylinder` |
| `fCapsule` | `Capsule` (axial or endpoint overload) |
| `fLineSegment` | `SegmentSurface` |
| `fTorus` | `Torus(majorRadius, minorRadius)`; reverse the source radii |
| `fCircle`, `fDisc` | `CircleCurve` / `DiscSurface`, now in XY; rotate into XZ if required |
| `fCone` | `Cone(radius, height / 2)`; translate up by height / 2 to retain the original base position |
| `fHexagonCircumcircle` | `HexagonalPrismBound(sourceRadius, halfHeight)` |
| `fHexagonIncircle` | `HexagonalPrismBound(sourceRadius * sqrt(3) / 2, halfHeight)` |
| `fGDF` | `PolyhedronBound` with inclusive direction indices and optional exponent |
| `fOctahedron` | `OctahedronBound(sourceRadius * sqrt(3))`, or `OctahedronPowerBound` for the exponent overload |
| `fDodecahedron`, `fIcosahedron`, `fTruncatedOctahedron`, `fTruncatedIcosahedron` | Corresponding `*Bound` constructors, with optional exponent |
| `pR`, `pR45` | `RotateZ(expression, radians)` / `RotateZ(expression, PI / 4)` for XY rotation |
| `pMod1` | `RepeatX` with half the source spacing |
| `pModMirror1` | `RepeatMirrorX` |
| `pModSingle1` | `RepeatPositiveX` |
| `pModInterval1` | `RepeatIntervalX` |
| `pModPolar` | `RepeatPolarZ` for XY, `RepeatPolarY` for XZ |
| `pMod2` | `RepeatXY` |
| `pModMirror2` | `RepeatMirrorXY` |
| `pModGrid2` | `RepeatGridXY` |
| `pMod3` | `Repeat` |
| `pMirror` | `MirrorX/Y/Z` |
| `pMirrorOctant` | `MirrorOctantXY` |
| `pReflect` | `FoldPlane(expression, normal, -sourceOffset)` |
| `fOpUnionChamfer`, `fOpIntersectionChamfer`, `fOpDifferenceChamfer` | `UnionChamfer` / `IntersectChamfer` / `SubtractChamfer` |
| `fOpUnionRound`, `fOpIntersectionRound`, `fOpDifferenceRound` | `UnionRound` / `IntersectRound` / `SubtractRound` |
| `fOpUnionColumns`, `fOpIntersectionColumns`, `fOpDifferenceColumns` | `UnionColumns` / `IntersectColumns` / `SubtractColumns` |
| `fOpUnionStairs`, `fOpIntersectionStairs`, `fOpDifferenceStairs` | `UnionStairs` / `IntersectStairs` / `SubtractStairs` |
| `fOpUnionSoft` | Existing `Blend` |
| `fOpPipe`, `fOpEngrave`, `fOpGroove`, `fOpTongue` | `Pipe` / `Engrave` / `Groove` / `Tongue` |

Repetition and fold overloads expose cell/side identifiers through `out`.
These remain expressions evaluated per sample, including after parameter updates.

```csharp
var part = shape.Translate(shape.Capsule(radius, 0.3f), new Vector3(0.65f, 0, 0));
return shape.RepeatPolarY(part, count, out var sector);
```

The sample scene `MercurySdf.unity` retains its scene and component identities,
but its scripts now use builder methods. The primitive component retains the
interactive twisted dodecahedron experiment. Rounded, chamfered, column, stair
and polar examples use the same API. `MercurySdfValidation.Run()` checks the six
meshes and live parameter changes in the Editor.

Named joins retain Mercury's assumptions about approximately perpendicular input
gradients. Polyhedron variants and the decorative blob are not promoted to exact
distance constructors. Formula reference tests continue to compare all 74 original
overloads, including coordinate changes and returned identifiers.
