# Migrating Inigo Quilez distance functions

Package 0.7.0 consolidates the IQ 2D/3D distance-function functionality into
`ShapeBuilder`. The separate public `IqSdf` class is removed. Source articles:
https://iquilezles.org/articles/distfunctions/ and
https://iquilezles.org/articles/distfunctions2d/. Attribution is retained in
[ThirdPartyNotices.md](ThirdPartyNotices.md).

See [ShapeBuilder modelling](SdfModelling.md) for the complete public API and
its parameter constraints. The original formulas remain internal implementation
details, with reference tests against their source conventions.

| Source function | Builder API |
| --- | --- |
| `Point` | `shape.Position` |
| `sdSphere` | Existing `Sphere` |
| `sdBox` | `BoxExact` (3D) / `RectangleExact` (2D) |
| `sdRoundBox`, `sdRoundedBox` | `RoundedBox` / `RoundedRectangle` |
| `sdChamferBox` | `ChamferedRectangle` |
| `sdOrientedBox` | `OrientedRectangle(start, end, sourceWidth / 2)` |
| `sdBoxFrame` | `BoxFrame` |
| `sdTorus` | Existing `Torus(majorRadius, minorRadius)` |
| `sdCappedTorus` | `CappedTorus` with half angle in radians; now a Y-axis torus |
| `sdLink` | `Link(majorRadius, minorRadius, halfLength)` |
| `sdCylinder` | `InfiniteCylinder` plus `Translate` for its axis offset |
| `sdCone` | `Cone(radius, halfHeight)` / `InfiniteCone(halfAngle)` |
| `sdPlane` | `Plane(normal, -sourceOffset)` |
| `sdHexPrism` | `HexagonalPrism`, now along Y |
| `sdCapsule`, `sdVerticalCapsule` | `Capsule`; axial centers are now at +/-halfLength |
| `sdCappedCylinder` | `CylinderExact(radius, halfHeight)` or endpoint `Cylinder` |
| `sdRoundedCylinder` | `RoundedCylinder(radius, halfHeight, roundingRadius)` |
| `sdCappedCone` | `Frustum` (axial or endpoint overload) |
| `sdRoundCone` | `RoundedCone` (axial or endpoint overload) |
| `sdSolidAngle` | `SolidAngle(radius, halfAngle)` |
| `sdCutSphere`, `sdCutHollowSphere` | `CutSphere` / `CutHollowSphere` |
| `sdDeathStar` | `SphereCavity(radius, cavityRadius, centerDistance)` |
| `sdVesicaSegment` | `VesicaSegment` |
| `sdRhombus` | `Rhombus` (2D) / `RhombusPrism` (3D) |
| `sdOctahedron`, `sdOctahedronBound` | `Octahedron` / `OctahedronBound` |
| `sdPyramid` | `Pyramid(halfWidth, halfHeight)`, base at -halfHeight |
| `udTriangle`, `udQuad` | `TriangleSurface` / `QuadSurface` |
| `sdEllipsoid`, `sdTriPrism` | `EllipsoidBound` / `TriangularPrismBound` (now along Y) |
| `sdCircle` | Existing `Circle` |
| `sdSegment` | `Segment` |
| `sdTrapezoid`, `sdParallelogram` | `Trapezoid` / `Parallelogram` |
| `sdEquilateralTriangle`, `sdTriangleIsosceles`, `sdTriangle` | `EquilateralTriangle` / `IsoscelesTriangle` / `Triangle` |
| `sdUnevenCapsule` | `UnevenCapsule`, centers now at +/-halfLength |
| `sdPentagon`, `sdHexagon`, `sdOctogon` | `Pentagon` / `Hexagon` / `Octagon` |
| `sdHexagram`, `sdPentagram`, `sdStar` | `Hexagram` / `Pentagram` / `Star`; star point count can now be a parameter |
| `sdPie`, `sdCutDisk`, `sdArc`, `sdRing`, `sdHorseshoe` | `Pie` / `CutCircle` / `Arc` / `RingSector` / `Horseshoe`; angle parameters are radians |
| `sdVesica`, `sdOrientedVesica` | `Vesica` with dimensional or endpoint overloads |
| `sdMoon` | `Moon(radius, cutoutRadius, centerDistance)` |
| `sdRoundedCross`, `sdEgg`, `sdHeart`, `sdCross` | `RoundedCross` / `Egg` / `Heart` / `CrossBound` |
| `sdRoundedX` | `RoundedX(outerHalfExtent, radius)` preserves outer bounds |
| `sdPolygon`, `sdEllipse` | `Polygon` / `Ellipse` |
| `sdParabola` | `Parabola` / `ParabolaSegment` |
| `sdBezier` | `Bezier` |
| `sdBlobbyCross`, `sdTunnel`, `sdStairs` | `BlobbyCross` / `Tunnel` / `Stairs` |
| `sdQuadraticCircle`, `sdHyberbola`, `sdfCoolS`, `sdCircleWave` | `QuadraticCircle` / `Hyperbola` / `CoolS` / `CircleWave` |
| `opRevolution`, `opExtrusion` | `RevolveY` / `ExtrudeZExact(expression, lower, upper)` |
| `opElongate`, `opElongateInterior` | `Elongate` / `ElongateInterior` |
| `opRound`, `opOnion` | `Round` / `Shell` |
| `length2`, `length6`, `length8` | `vector.Length()` / `LengthPower(6)` / `LengthPower(8)` |
| `opUnion`, `opIntersection`, `opSubtraction`, `opXor` | `Union` / `Intersect` / `Subtract` / `Xor`; reverse IQ subtraction operands |
| `opSmoothUnion`, `opSmoothIntersection`, `opSmoothSubtraction` | `Blend` / `IntersectBlend` / `SubtractBlend`, with radius = 4 * source amount; reverse IQ subtraction operands |
| `opTx`, `opScale` | Existing `Transform` / new `ScaleUniformExact` |
| `opSymX`, `opSymXZ` | `SymmetryX` / `SymmetryXZ` |
| `opRepetition`, `opLimitedRepetition` | `Repeat` / `RepeatLimited` with per-axis spacing/limits and optional identifiers |
| `opDisplace`, `opTwist`, `opCheapBend` | `Displace` / `TwistY` / `BendZ` |

```csharp
var profile = shape.Star(radius, points, 3);
return shape.ExtrudeZExact(profile, -0.2f, 0.2f);
```

The source's sine/cosine angle pairs become one angle in radians. No callback
is needed to extrude, transform or deform an existing expression. Rotation and
`Remap` can recover source orientations when they differ from the Y-axis-solid,
XY-profile conventions.

`Cone` is centered with base at -halfHeight and tip at +halfHeight. The source
finite cone points downward from its tip at zero: rotate the centered cone by
PI about X and translate by -halfHeight along Y to reproduce that placement.
`RoundedCone` and `UnevenCapsule` use centered endpoint positions instead of
source endpoints at zero and height. `Pyramid` also uses centered axial bounds.

The supplied vesica declaration typo is corrected internally. Rhombus axial-tip
signs avoid spurious zero-distance rays; tall vesicas swap profile orientation.
Endpoint cylinders and spherical sectors use derivative-preserving equivalents. Twist preserves Y.
Ellipse equal radii/center, straight Bezier curves and the parabola zero-q case
retain the numerical guards added during the port. The ellipsoid bound retains
the source's undefined origin limit. Closed-form curves can remain ill-conditioned
near repeated roots. IQ's spelling errors are corrected in the public builder names.

Reference checks still compile all 102 original overloads and probe 6,120 seeded
samples; independent Euclidean distances and numerical ellipse/Bezier closest
points validate the formulas. Builder tests separately check the new conventions,
parameterized counts, identifiers, and native meshes. The Editor integration
probe `IntegrationCheck.RunModelling()` checks the public affine transform and
worker meshing path.
