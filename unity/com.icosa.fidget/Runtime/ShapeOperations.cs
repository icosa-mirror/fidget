// Distance formulas adapted from Inigo Quilez and Mercury hg_sdf.
// See ThirdPartyNotices.md.
using UnityEngine;

namespace Icosa.Fidget
{
    public partial class ShapeBuilder
    {
        /// <summary>Offset the field outward. A geometric rounding radius requires an exact input distance field.</summary>
        public Expression Round(Expression shape, Expression radius) { Import(shape); return shape - radius; }
        /// <summary>Give a surface thickness on either side. Requires an exact input for uniform geometric thickness.</summary>
        public Expression Shell(Expression shape, Expression halfThickness) { Import(shape); return shape.Abs() - halfThickness; }
        /// <summary>Uniform scale with field compensation. Scale must remain nonzero; exactness requires exact input.</summary>
        public Expression ScaleUniformExact(Expression shape, Expression scale) => ScaleUniform(shape, scale) * scale.Abs();
        /// <summary>Extrude an exact XY profile with Euclidean distance around the end edges. Upper must exceed lower.</summary>
        public Expression ExtrudeZExact(Expression shape, Expression lower, Expression upper)
        {
            var profile = Remap(shape, X, Y, 0);
            var cap = (lower - Z).Max(Z - upper);
            return profile.Max(cap).Min(0) + Length(new Expression2(profile.Max(0), cap.Max(0)));
        }
        /// <summary>Symmetric difference. Interior distances may be bounds.</summary>
        public Expression Xor(Expression a, Expression b) { Import(a); Import(b); return a.Min(b).Max(-a.Max(b)); }
        /// <summary>Quadratic smooth intersection using the same radius convention as Blend. Produces a bound.</summary>
        public Expression IntersectBlend(Expression a, Expression b, Expression radius) => -Blend(-a, -b, radius);
        /// <summary>Quadratic smooth subtraction, shape minus cutout. Produces a bound.</summary>
        public Expression SubtractBlend(Expression shape, Expression cutout, Expression radius) => -Blend(-shape, cutout, radius);
        public Expression UnionChamfer(Expression a, Expression b, Expression radius) => HgSdf.fOpUnionChamfer(a, b, radius);
        public Expression IntersectChamfer(Expression a, Expression b, Expression radius) => HgSdf.fOpIntersectionChamfer(a, b, radius);
        public Expression SubtractChamfer(Expression shape, Expression cutout, Expression radius) => HgSdf.fOpDifferenceChamfer(shape, cutout, radius);
        public Expression UnionRound(Expression a, Expression b, Expression radius) => HgSdf.fOpUnionRound(a, b, radius);
        public Expression IntersectRound(Expression a, Expression b, Expression radius) => HgSdf.fOpIntersectionRound(a, b, radius);
        public Expression SubtractRound(Expression shape, Expression cutout, Expression radius) => HgSdf.fOpDifferenceRound(shape, cutout, radius);
        /// <summary>Mercury column join. Radius must be positive and count >= 2; best with perpendicular input gradients.</summary>
        public Expression UnionColumns(Expression a, Expression b, Expression radius, Expression count) => HgSdf.fOpUnionColumns(a, b, radius, count);
        public Expression IntersectColumns(Expression a, Expression b, Expression radius, Expression count) => HgSdf.fOpIntersectionColumns(a, b, radius, count);
        public Expression SubtractColumns(Expression shape, Expression cutout, Expression radius, Expression count) => HgSdf.fOpDifferenceColumns(shape, cutout, radius, count);
        public Expression UnionStairs(Expression a, Expression b, Expression radius, Expression count) => HgSdf.fOpUnionStairs(a, b, radius, count);
        public Expression IntersectStairs(Expression a, Expression b, Expression radius, Expression count) => HgSdf.fOpIntersectionStairs(a, b, radius, count);
        public Expression SubtractStairs(Expression shape, Expression cutout, Expression radius, Expression count) => HgSdf.fOpDifferenceStairs(shape, cutout, radius, count);
        public Expression Pipe(Expression a, Expression b, Expression radius) => HgSdf.fOpPipe(a, b, radius);
        public Expression Engrave(Expression shape, Expression cutter, Expression radius) => HgSdf.fOpEngrave(shape, cutter, radius);
        public Expression Groove(Expression shape, Expression cutter, Expression depth, Expression halfWidth) => HgSdf.fOpGroove(shape, cutter, depth, halfWidth);
        public Expression Tongue(Expression shape, Expression guide, Expression height, Expression halfWidth) => HgSdf.fOpTongue(shape, guide, height, halfWidth);
        /// <summary>Twist around Y; amount is radians per Y unit. Distorts distance values.</summary>
        public Expression TwistY(Expression shape, Expression radiansPerUnit)
        {
            var angle = radiansPerUnit * Y; var c = angle.Cos(); var s = angle.Sin();
            return Remap(shape, c * X + s * Z, Y, -s * X + c * Z);
        }
        /// <summary>Bend XY around Z by radiansPerUnit * X. Distorts distance values.</summary>
        public Expression BendZ(Expression shape, Expression radiansPerUnit)
        {
            var angle = radiansPerUnit * X; var c = angle.Cos(); var s = angle.Sin();
            return Remap(shape, c * X + s * Y, -s * X + c * Y, Z);
        }
        /// <summary>Elongate by nonnegative half extents; multi-axis elongation can have a zero-valued interior core.</summary>
        public Expression Elongate(Expression shape, Expression3 halfExtents) => Remap(shape,
            X - X.Max(-halfExtents.x).Min(halfExtents.x), Y - Y.Max(-halfExtents.y).Min(halfExtents.y), Z - Z.Max(-halfExtents.z).Min(halfExtents.z));
        /// <summary>Elongation with interior correction, for a centered primitive symmetric about its coordinate planes.</summary>
        public Expression ElongateInterior(Expression shape, Expression3 halfExtents)
        {
            var q = new Expression3(X.Abs(), Y.Abs(), Z.Abs()) - halfExtents;
            return Remap(shape, q.x.Max(0), q.y.Max(0), q.z.Max(0)) + q.x.Max(q.y).Max(q.z).Min(0);
        }
        /// <summary>Add a displacement field. The result is generally not a distance bound.</summary>
        public Expression Displace(Expression shape, Expression displacement) { Import(shape); Import(displacement); return shape + displacement; }
        /// <summary>Copy the positive side across X=0. Cross-boundary input shapes may lose distance accuracy.</summary>
        public Expression SymmetryX(Expression shape) => Remap(shape, X.Abs(), Y, Z);
        public Expression SymmetryY(Expression shape) => Remap(shape, X, Y.Abs(), Z);
        public Expression SymmetryZ(Expression shape) => Remap(shape, X, Y, Z.Abs());
        public Expression SymmetryXZ(Expression shape) => Remap(shape, X.Abs(), Y, Z.Abs());
        /// <summary>Reflect only the negative side of dot(normal,position)=offset. Normal is normalized.</summary>
        public Expression FoldPlane(Expression shape, Vector3 normal, Expression offset) => FoldPlane(shape, normal, offset, out _);
        public Expression FoldPlane(Expression shape, Vector3 normal, Expression offset, out Expression side)
        {
            var p = Position;
            side = HgSdf.pReflect(ref p, UnitAxis(normal), -offset);
            return Remap(shape, p.x, p.y, p.z);
        }
        public Expression MirrorX(Expression shape, Expression offset = default) => MirrorX(shape, offset, out _);
        public Expression MirrorX(Expression shape, Expression offset, out Expression side)
        {
            var x = X; side = HgSdf.pMirror(ref x, offset); return Remap(shape, x, Y, Z);
        }
        public Expression MirrorY(Expression shape, Expression offset = default) => MirrorY(shape, offset, out _);
        public Expression MirrorY(Expression shape, Expression offset, out Expression side)
        {
            var y = Y; side = HgSdf.pMirror(ref y, offset); return Remap(shape, X, y, Z);
        }
        public Expression MirrorZ(Expression shape, Expression offset = default) => MirrorZ(shape, offset, out _);
        public Expression MirrorZ(Expression shape, Expression offset, out Expression side)
        {
            var z = Z; side = HgSdf.pMirror(ref z, offset); return Remap(shape, X, Y, z);
        }
        /// <summary>Fold XY into an octant, after mirroring and subtracting the offset.</summary>
        public Expression MirrorOctantXY(Expression shape, Expression2 offset) => MirrorOctantXY(shape, offset, out _);
        public Expression MirrorOctantXY(Expression shape, Expression2 offset, out Expression2 sides)
        {
            var p = ProfilePosition; sides = HgSdf.pMirrorOctant(ref p, offset); return Remap(shape, p.x, p.y, Z);
        }
        /// <summary>Repeat at full cell spacing. Shapes must match at tile boundaries for correct distance bounds.</summary>
        public Expression Repeat(Expression shape, Expression3 spacing) => Repeat(shape, spacing, out _);
        public Expression Repeat(Expression shape, Expression3 spacing, out Expression3 cell)
        {
            var p = Position; cell = HgSdf.pMod3(ref p, spacing); return Remap(shape, p.x, p.y, p.z);
        }
        // Existing RepeatX uses a half-spacing radius; new axis methods retain that convention.
        public Expression RepeatX(Expression shape, Expression radius, Expression offset, out Expression cell)
        {
            var x = X - offset; cell = HgSdf.pMod1(ref x, 2 * radius); return Remap(shape, x + offset, Y, Z);
        }
        public Expression RepeatY(Expression shape, Expression radius, Expression offset = default) => RepeatY(shape, radius, offset, out _);
        public Expression RepeatY(Expression shape, Expression radius, Expression offset, out Expression cell)
        {
            var y = Y - offset; cell = HgSdf.pMod1(ref y, 2 * radius); return Remap(shape, X, y + offset, Z);
        }
        public Expression RepeatZ(Expression shape, Expression radius, Expression offset = default) => RepeatZ(shape, radius, offset, out _);
        public Expression RepeatZ(Expression shape, Expression radius, Expression offset, out Expression cell)
        {
            var z = Z - offset; cell = HgSdf.pMod1(ref z, 2 * radius); return Remap(shape, X, Y, z + offset);
        }
        public Expression RepeatXY(Expression shape, Expression2 spacing) => RepeatXY(shape, spacing, out _);
        public Expression RepeatXY(Expression shape, Expression2 spacing, out Expression2 cell)
        {
            var p = ProfilePosition; cell = HgSdf.pMod2(ref p, spacing); return Remap(shape, p.x, p.y, Z);
        }
        public Expression RepeatMirrorXY(Expression shape, Expression2 spacing) => RepeatMirrorXY(shape, spacing, out _);
        public Expression RepeatMirrorXY(Expression shape, Expression2 spacing, out Expression2 cell)
        {
            var p = ProfilePosition; cell = HgSdf.pModMirror2(ref p, spacing); return Remap(shape, p.x, p.y, Z);
        }
        public Expression RepeatGridXY(Expression shape, Expression2 spacing) => RepeatGridXY(shape, spacing, out _);
        public Expression RepeatGridXY(Expression shape, Expression2 spacing, out Expression2 cell)
        {
            var p = ProfilePosition; cell = HgSdf.pModGrid2(ref p, spacing); return Remap(shape, p.x, p.y, Z);
        }
        public Expression RepeatMirrorX(Expression shape, Expression spacing) => RepeatMirrorX(shape, spacing, out _);
        public Expression RepeatMirrorX(Expression shape, Expression spacing, out Expression cell)
        {
            var x = X; cell = HgSdf.pModMirror1(ref x, spacing); return Remap(shape, x, Y, Z);
        }
        /// <summary>Repeat only on the positive X side; spacing is the full cell width.</summary>
        public Expression RepeatPositiveX(Expression shape, Expression spacing) => RepeatPositiveX(shape, spacing, out _);
        public Expression RepeatPositiveX(Expression shape, Expression spacing, out Expression cell)
        {
            var x = X; cell = HgSdf.pModSingle1(ref x, spacing); return Remap(shape, x, Y, Z);
        }
        public Expression RepeatIntervalX(Expression shape, Expression spacing, Expression firstCell, Expression lastCell) => RepeatIntervalX(shape, spacing, firstCell, lastCell, out _);
        public Expression RepeatIntervalX(Expression shape, Expression spacing, Expression firstCell, Expression lastCell, out Expression cell)
        {
            var x = X; cell = HgSdf.pModInterval1(ref x, spacing, firstCell, lastCell); return Remap(shape, x, Y, Z);
        }
        /// <summary>Finite repetition from -limit to +limit per axis. Limits and spacing must stay nonnegative/positive.</summary>
        public Expression RepeatLimited(Expression shape, Expression3 spacing, Expression3 limit) => RepeatLimited(shape, spacing, limit, out _);
        public Expression RepeatLimited(Expression shape, Expression3 spacing, Expression3 limit, out Expression3 cell)
        {
            Expression Index(Expression p, Expression s, Expression l) => ((p / s).Abs() + 0.5f).Floor() * (p / s).Compare(0).Max(-1).Min(1);
            cell = new Expression3(Index(X, spacing.x, limit.x).Max(-limit.x).Min(limit.x), Index(Y, spacing.y, limit.y).Max(-limit.y).Min(limit.y), Index(Z, spacing.z, limit.z).Max(-limit.z).Min(limit.z));
            var p = Position - spacing * cell; return Remap(shape, p.x, p.y, p.z);
        }
        /// <summary>Repeat about Y in XZ. Count must remain a positive integer; cell is the angular sector identifier.</summary>
        public Expression RepeatPolarY(Expression shape, Expression count) => RepeatPolarY(shape, count, out _);
        public Expression RepeatPolarY(Expression shape, Expression count, out Expression cell)
        {
            var p = new Expression2(X, Z); cell = HgSdf.pModPolar(ref p, count); return Remap(shape, p.x, Y, p.y);
        }
        public Expression RepeatPolarZ(Expression shape, Expression count) => RepeatPolarZ(shape, count, out _);
        public Expression RepeatPolarZ(Expression shape, Expression count, out Expression cell)
        {
            var p = ProfilePosition; cell = HgSdf.pModPolar(ref p, count); return Remap(shape, p.x, p.y, Z);
        }
    }
}
