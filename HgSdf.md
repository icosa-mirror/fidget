# Mercury hg_sdf for Unity

This is a formula port of Mercury's hg_sdf, version 2021-07-28:
https://mercury.sexy/hg_sdf/hg_sdf.glsl. Function names, parameter order, axes,
thresholds, and returned domain cell IDs follow the original. The port builds
Fidget expressions in C# and meshes their zero surfaces. It does not run GLSL.
Requires Fidget package 0.5.0 and matching ABI 6 native libraries.

## Component example

```csharp
using Icosa.Fidget;
using UnityEngine;

public class RoundedJoin : FidgetMesh<RoundedJoin>
{
    [field: SerializeField, Range(0.05f, 0.5f)]
    public float Radius { get; set; } = 0.2f;

    protected override Expression Define(ShapeBuilder<RoundedJoin> shape)
    {
        var p = HgSdf.Point(shape);
        var a = HgSdf.fBox(p, new Expression3(0.7f, 0.25f, 0.4f));
        var b = HgSdf.fBox(p - new Expression3(-0.4f, 0.4f, 0),
                          new Expression3(0.25f, 0.7f, 0.4f));
        return HgSdf.fOpUnionRound(a, b, shape.Parameter(mesh => mesh.Radius));
    }
}
```

`Expression2`, `Expression3`, and `Expression4` contain scalar expressions and
support component arithmetic. Unity `Vector2/3/4` values convert implicitly.
Use `new Expression3(value)` to broadcast a scalar. Lowercase fields and swizzles
match shader notation. All angles are radians.

## Domain operators

The original `inout` parameter becomes a C# `ref` parameter. These methods change
coordinate expressions once during graph construction. Their effects and returned
cell IDs are evaluated for every sampled point, including after parameter updates.

```csharp
var p = HgSdf.Point(shape);
var radial = p.xz;
var cell = HgSdf.pModPolar(ref radial, shape.Parameter(mesh => mesh.Count));
p.xz = radial;
p.x -= 0.6f;
return HgSdf.fCylinder(p, 0.12f, 0.4f);
```

Extract a swizzle before passing it by `ref`, then assign it back, as above.
Individual fields such as `p.x` can be passed directly by `ref`. Returned cell IDs
are `Expression` values (or expression vectors), so they can participate in
subsequent graph arithmetic. They cannot be used as ordinary C# `if` conditions.

## Function reference

| Category | Functions |
| --- | --- |
| Helpers | `saturate`, `sgn`, `square`, `lengthSqr`, `vmax`, `vmin` |
| Basic primitives | `fSphere`, `fPlane`, `fBox`, `fBoxCheap`, `fBox2`, `fBox2Cheap`, `fCorner`, `fBlob` |
| Curved primitives | `fCylinder`, both `fCapsule` overloads, `fLineSegment`, `fTorus`, `fCircle`, `fDisc`, `fCone` |
| Prisms/polyhedra | `fHexagonCircumcircle`, `fHexagonIncircle`, both `fGDF` overloads; `fOctahedron`, `fDodecahedron`, `fIcosahedron`, `fTruncatedOctahedron`, `fTruncatedIcosahedron`, each with optional exponent |
| Rotations/folding | `pR`, `pR45`, `pMirror`, `pMirrorOctant`, `pReflect` |
| Repetition | `pMod1`, `pModMirror1`, `pModSingle1`, `pModInterval1`, `pModPolar`, `pMod2`, `pModMirror2`, `pModGrid2`, `pMod3` |
| Chamfered CSG | `fOpUnionChamfer`, `fOpIntersectionChamfer`, `fOpDifferenceChamfer` |
| Rounded CSG | `fOpUnionRound`, `fOpIntersectionRound`, `fOpDifferenceRound`, `fOpUnionSoft` |
| Patterned CSG | `fOpUnionColumns`, `fOpIntersectionColumns`, `fOpDifferenceColumns`, `fOpUnionStairs`, `fOpIntersectionStairs`, `fOpDifferenceStairs` |
| Features | `fOpPipe`, `fOpEngrave`, `fOpGroove`, `fOpTongue` |

## Conventions

- `fBox` uses half extents and correct corner distances. `fBoxCheap` matches the
  existing builder's `Box` field. Keep these distinct when using rounded joins.
- `fCylinder` takes a radius and half height. `fCone` has its base at Y = 0 and
  tip at Y = height. Most other primitives are centered at the origin with Y up.
- `fTorus` takes **small radius first, large radius second**, following Mercury.
  This differs from the existing builder's `Torus` argument order.
- `fCircle` is unsigned distance to a circle line in XZ; subtract a tube radius to
  give it volume. `fDisc` has zero thickness; subtract a thickness to mesh it as
  a solid. Neither is the builder's filled XY `Circle` profile.
- `fPlane` and `pReflect` expect a normalized normal and use Mercury's additive
  offset convention. `pReflect` folds one half-space rather than reflecting the
  whole object like the builder's `Reflect`.
- Use positive dimensions, nonzero repetition sizes, positive radii and positive
  GDF exponents. Column counts must exceed 1; stairs/polar counts must be positive.
  Integer counts give the intended repeating patterns. Capsule endpoints must
  differ. GDF direction indices are inclusive and range from 0 through 18.
- Upstream mathematical behavior is retained, including `fBlob`'s singularity
  at the origin and the discontinuities of some domain/column operators. These
  can affect meshing and surface gradients. Keep features resolvable at the
  selected depth and the complete desired surface inside `Bounds`.
- These functions return fields, not always exact signed distances. Mercury's
  rounded/chamfered combinations assume approximately perpendicular input
  gradients for their distance-bound guarantees; their formulas are preserved.

## Sample and verification

Open `Scenes/MercurySdf.unity` from the Examples sample. Its six focused
`FidgetMesh<T>` components show primitives, rounded and chamfered joins, columns,
stairs, and polar repetition. Press Play and change their Inspector properties.

The graph checks compare every ported overload with a numeric translation of the
pinned original GLSL, including domain mutations and returned cell IDs. The
reference generator preserves the original numeric control flow. Native VM tests
cover the added operations and unselected undefined branches. The Editor probe
`FidgetExamples.Editor.MercurySdfValidation.Run()` checks all six meshes, normals,
parameter updates, and a rendered preview. This does not establish player or
mobile-device performance.

Copyright (c) 2011-2021 Mercury Demogroup. The port uses the MIT option; the full
notice is included in [ThirdPartyNotices.md](ThirdPartyNotices.md).
