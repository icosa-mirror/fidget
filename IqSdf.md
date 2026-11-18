# Inigo Quilez distance functions for Unity

`IqSdf` is a separate expression-building library based on the 2D and 3D source
articles supplied by the user:

- https://iquilezles.org/articles/distfunctions/
- https://iquilezles.org/articles/distfunctions2d/

The formulas are credited to Inigo Quilez. Names and argument order follow those
articles, including deliberate overlap with `HgSdf`. The two APIs remain separate
because their coordinate, sign, distance, and operator conventions differ.
Requires package 0.6.0 with matching ABI 6 native libraries.

## Use from a component

```csharp
using Icosa.Fidget;
using UnityEngine;

public class StarPrism : FidgetMesh<StarPrism>
{
    [field: SerializeField, Range(0.2f, 0.9f)]
    public float Radius { get; set; } = 0.65f;

    protected override Expression Define(ShapeBuilder<StarPrism> shape)
    {
        var p = IqSdf.Point(shape);
        var radius = shape.Parameter(mesh => mesh.Radius);
        return IqSdf.opExtrusion(p,
            q => IqSdf.sdStar(q, radius, 5, 3), 0.2f);
    }
}
```

`Expression2`, `Expression3`, and `Expression4` hold expression components. Unity
vectors convert implicitly; `new Expression3(value)` broadcasts a scalar. All
coordinates are local model coordinates, all angles are radians, and negative
field values indicate interior for signed primitives. `udTriangle`, `udQuad`,
segments and Bezier curves return unsigned distances; subtract a thickness to
make them volumetric before meshing.

## 3D functions

| Group | Functions |
| --- | --- |
| Basic solids | `sdSphere`, `sdBox`, `sdRoundBox`, `sdBoxFrame`, `sdPlane` |
| Rings | `sdTorus`, `sdCappedTorus`, `sdLink` |
| Cylinders/capsules | `sdCylinder` (infinite), `sdCapsule`, `sdVerticalCapsule`, both `sdCappedCylinder` overloads, `sdRoundedCylinder` |
| Cones | both `sdCone` overloads (finite/infinite), both `sdCappedCone` overloads (frusta), both `sdRoundCone` overloads (tapered round-ended shapes) |
| Spherical shapes | `sdSolidAngle`, `sdCutSphere`, `sdCutHollowSphere`, `sdDeathStar`, `sdVesicaSegment` |
| Polyhedra | `sdHexPrism`, `sdRhombus`, `sdOctahedron`, `sdOctahedronBound`, `sdPyramid` |
| Surface distances | `udTriangle`, `udQuad` |
| Bounds | `sdEllipsoid`, `sdTriPrism` |

## 2D functions

| Group | Functions |
| --- | --- |
| Basic profiles | `sdCircle`, `sdBox`, `sdRoundedBox`, `sdChamferBox`, `sdOrientedBox`, `sdSegment` |
| Polygons | `sdRhombus`, `sdTrapezoid`, `sdParallelogram`, `sdEquilateralTriangle`, `sdTriangleIsosceles`, `sdTriangle`, `sdPentagon`, `sdHexagon`, `sdOctogon`, `sdPolygon` |
| Stars | `sdHexagram`, `sdPentagram`, `sdStar` |
| Circular profiles | `sdPie`, `sdCutDisk`, `sdArc`, `sdRing`, `sdHorseshoe`, `sdVesica`, `sdOrientedVesica`, `sdMoon`, `sdCircleWave` |
| Shaped profiles | `sdUnevenCapsule`, `sdRoundedCross`, `sdEgg`, `sdHeart`, `sdCross`, `sdRoundedX`, `sdBlobbyCross`, `sdTunnel`, `sdStairs`, `sdfCoolS` |
| Curves | `sdEllipse`, both `sdParabola` overloads, `sdBezier`, `sdQuadraticCircle`, `sdHyberbola` |

`sdOctogon` and `sdHyberbola` retain the spellings in the supplied article.
`sdPolygon` accepts an `Expression2[]`; the array length is graph structure while
its vertex coordinates may contain bound parameters.

## Operators

| Purpose | Functions |
| --- | --- |
| From profiles | `opRevolution`, `opExtrusion` |
| Shape modification | `opElongate`, `opElongateInterior`, `opRound`, `opOnion` |
| Alternate metrics | `length2`, `length6`, `length8` |
| Boolean | `opUnion`, `opSubtraction`, `opIntersection`, `opXor` |
| Smooth combinations | `opSmoothUnion`, `opSmoothSubtraction`, `opSmoothIntersection` |
| Placement | `opTx`, `opScale` |
| Symmetry/repetition | `opSymX`, `opSymXZ`, `opRepetition`, `opLimitedRepetition` |
| Deformation | `opDisplace`, `opTwist`, `opCheapBend` |

Operators taking `Func<Expression2, Expression>` or
`Func<Expression3, Expression>` invoke it while constructing the graph, not once
per sampled point. Bound model properties still update without rebuilding.
`opDisplace` takes an explicit displacement function. `opTwist` and `opCheapBend`
take an explicit strength instead of the article's illustrative constant 10.
`opRound` and `opOnion` accept an already-built scalar field.

## Conventions and adjustments

- IQ's `opSubtraction(a, b)` subtracts **a from b**. The builder's
  `Subtract(a, b)` and Mercury's difference operators subtract b from a.
- IQ uses GLSL `sign(0) = 0`. Mercury's `sgn(0) = 1` is different.
- `sdRoundedBox` in 2D takes four corner radii: the supplied article's quadrant
  selection is retained. Box sizes are half extents. `sdRoundBox` in 3D keeps
  the supplied outer half extents while rounding its corners.
- `sdCappedCylinder` and `sdHexPrism` provide exact exterior corner distances;
  Mercury's corresponding fields are cheaper bounds at those corners.
- `opExtrusion` preserves the profile's Euclidean distances at the end-cap
  edges. The existing builder's `ExtrudeZ` uses the simpler maximum field.
- IQ smooth combinations rescale `k` by four internally. Their parameter is not
  interchangeable with Mercury's `fOpUnionSoft` radius.
- `opTx` takes a fixed invertible affine local-to-model matrix. Use only rigid
  transforms for unchanged Euclidean distances. `opScale` expects a positive
  uniform scale and corrects the resulting distance magnitude.
- The duplicate exact/bound octahedron functions become `sdOctahedron` and
  `sdOctahedronBound`. The two elongation forms become `opElongate` and
  `opElongateInterior`.
- The supplied vesica snippet's `vec3 d` declaration is corrected to a scalar.
  `opTwist` keeps Y unchanged while rotating XZ; the supplied snippet's tuple
  ordering would exchange Y and Z even at zero twist.
- Ellipse equal radii reduce to a circle; its center returns the negative minor
  radius. A straight quadratic Bezier reduces to a segment. The parabola cubic
  solver handles its zero-q case explicitly. These avoid undefined limits in
  the supplied closed-form code.
- Rounding uses nearest integer with ties away from zero, one of GLSL's allowed
  tie choices. Repeat symmetric cells or ensure fields match at cell boundaries.

Use positive dimensions, distinct segment endpoints, nondegenerate polygons and
triangles, normalized plane normals, and valid aperture sine/cosine pairs. Cone
heights and repetition spacings must be nonzero. Rounded-cone radii must differ
by less than the endpoint separation. Curve parameter ranges must describe a
valid curve. Avoid repeated polygon vertices/zero-length edges.

The source's non-exact fields remain non-exact; for example `sdEllipsoid` is a
bound and retains its undefined origin limit. Complex closed-form curves can be
numerically ill-conditioned near repeated roots or symmetry axes. These functions
describe fields; meshing still depends on resolution, explicit bounds, valid
field values, and meaningful surface gradients.

## Validation

The standalone checks construct and natively compile 102 overloads, probe their
finite values over seeded spatial samples, and check shared primitives against
independent distance formulas. Ellipse and Bezier distances are compared with
numerical closest-point minimization rather than another translation of their
closed-form solver. Six representative IQ solids are meshed with surface normals.
`opTx` additionally requires Unity's matrix implementation and is checked in the
Editor integration probe. This does not establish player/device performance.
