// Distance formulas adapted from Inigo Quilez and Mercury hg_sdf.
// See ThirdPartyNotices.md for attribution and the Mercury MIT notice.
using System;
using UnityEngine;

namespace Icosa.Fidget
{
    public partial class ShapeBuilder
    {
        /// <summary>Local model coordinate expressions, shared by all builder shapes.</summary>
        public Expression3 Position => new Expression3(X, Y, Z);
        private Expression2 ProfilePosition => new Expression2(X, Y);
        private static Expression Length(Expression3 p) => (p.x.Square() + p.y.Square() + p.z.Square()).Sqrt();
        private static Expression Length(Expression2 p) => (p.x.Square() + p.y.Square()).Sqrt();

        /// <summary>Euclidean signed distance to an axis-aligned box, specified by half extents.</summary>
        public Expression BoxExact(Expression x, Expression y, Expression z) => IqSdf.sdBox(Position, new Expression3(x, y, z));
        public Expression BoxExact(Expression3 halfExtents) => BoxExact(halfExtents.x, halfExtents.y, halfExtents.z);
        public Expression BoxExact(Vector3 lower, Vector3 upper) => Translate(BoxExact((Expression3)((upper - lower) * 0.5f)), (upper + lower) * 0.5f);
        /// <summary>Euclidean signed distance to an XY rectangle, specified by half extents.</summary>
        public Expression RectangleExact(Expression halfWidth, Expression halfHeight) => IqSdf.sdBox(ProfilePosition, new Expression2(halfWidth, halfHeight));
        public Expression RectangleExact(Expression2 halfExtents) => RectangleExact(halfExtents.x, halfExtents.y);
        public Expression RectangleExact(Vector2 lower, Vector2 upper) => Translate(RectangleExact((Expression2)((upper - lower) * 0.5f)), (lower.x + upper.x) * 0.5f, (lower.y + upper.y) * 0.5f, 0);
        /// <summary>Rounded box preserving outer half extents; radius must be between zero and the smallest half extent.</summary>
        public Expression RoundedBox(Expression3 halfExtents, Expression radius) => IqSdf.sdRoundBox(Position, halfExtents, radius);
        /// <summary>Rounded rectangle preserving outer half extents.</summary>
        public Expression RoundedRectangle(Expression2 halfExtents, Expression radius) => RoundedRectangle(halfExtents, new Expression4(radius));
        /// <summary>Corner radii in +X/+Y, +X/-Y, -X/+Y, -X/-Y order.</summary>
        public Expression RoundedRectangle(Expression2 halfExtents, Expression4 cornerRadii) => IqSdf.sdRoundedBox(ProfilePosition, halfExtents, cornerRadii);
        public Expression ChamferedRectangle(Expression2 halfExtents, Expression chamfer) => IqSdf.sdChamferBox(ProfilePosition, halfExtents, chamfer);
        public Expression OrientedRectangle(Expression2 start, Expression2 end, Expression halfWidth) => IqSdf.sdOrientedBox(ProfilePosition, start, end, 2 * halfWidth);
        public Expression BoxFrame(Expression3 halfExtents, Expression edgeWidth) => IqSdf.sdBoxFrame(Position, halfExtents, edgeWidth);
        /// <summary>Euclidean signed distance to a Y-axis capped cylinder.</summary>
        public Expression CylinderExact(Expression radius, Expression halfHeight) => IqSdf.sdCappedCylinder(Position, radius, halfHeight);
        /// <summary>Euclidean signed distance to a capped cylinder between distinct endpoints.</summary>
        public Expression Cylinder(Expression3 start, Expression3 end, Expression radius)
        {
            // Project onto the axis instead of signing a squared distance. The
            // latter has a zero-sign branch on planar caps and loses their normals.
            var axis = end - start;
            var length = axis.Length();
            var direction = axis / length;
            var relative = Position - (start + end) * 0.5f;
            var axial = relative.Dot(direction);
            var radial = (relative - direction * axial).Length() - radius;
            var cap = axial.Abs() - length * 0.5f;
            return radial.Max(cap).Min(0) + Length(new Expression2(radial.Max(0), cap.Max(0)));
        }
        public Expression InfiniteCylinder(Expression radius) => Length(new Expression2(X, Z)) - radius;
        /// <summary>Rounded cylinder preserving its outer radius and half height.</summary>
        public Expression RoundedCylinder(Expression radius, Expression halfHeight, Expression roundingRadius) => IqSdf.sdRoundedCylinder(Position, radius, roundingRadius, halfHeight);
        /// <summary>Y-axis cone, base at -halfHeight and tip at +halfHeight.</summary>
        public Expression Cone(Expression radius, Expression halfHeight) => Frustum(radius, 0, halfHeight);
        /// <summary>Y-axis frustum, lower and upper radii at -halfHeight and +halfHeight.</summary>
        public Expression Frustum(Expression lowerRadius, Expression upperRadius, Expression halfHeight) => IqSdf.sdCappedCone(Position, halfHeight, lowerRadius, upperRadius);
        public Expression Frustum(Expression3 start, Expression3 end, Expression startRadius, Expression endRadius) => IqSdf.sdCappedCone(Position, start, end, startRadius, endRadius);
        /// <summary>Infinite cone with tip at the origin, opening toward +Y; angle is in radians, between zero and pi/2.</summary>
        public Expression InfiniteCone(Expression halfAngle) => IqSdf.sdCone(new Expression3(X, -Y, Z), new Expression2(halfAngle.Sin(), halfAngle.Cos()));
        /// <summary>Capsule with sphere centers at Y = +/-halfLength.</summary>
        public Expression Capsule(Expression radius, Expression halfLength) => Length(new Expression3(X, (Y.Abs() - halfLength).Max(0), Z)) - radius;
        public Expression Capsule(Expression3 start, Expression3 end, Expression radius) => IqSdf.sdCapsule(Position, start, end, radius);
        /// <summary>Convex hull of spheres centered at Y = +/-halfLength; center separation must exceed the radius difference.</summary>
        public Expression RoundedCone(Expression lowerRadius, Expression upperRadius, Expression halfLength) => IqSdf.sdRoundCone(new Expression3(X, Y + halfLength, Z), lowerRadius, upperRadius, 2 * halfLength);
        public Expression RoundedCone(Expression3 start, Expression3 end, Expression startRadius, Expression endRadius) => IqSdf.sdRoundCone(Position, start, end, startRadius, endRadius);
        /// <summary>Signed distance to dot(normalized normal, position) = offset.</summary>
        public Expression Plane(Vector3 normal, Expression offset = default) => Plane((Expression3)UnitAxis(normal), offset);
        /// <summary>Expression-valued normal; its length must stay nonzero. Offset is measured along the normalized normal.</summary>
        public Expression Plane(Expression3 normal, Expression offset = default) => IqSdf.sdPlane(Position, normal / normal.Length(), -offset);
        /// <summary>Y-axis torus arc centered toward +Z. Aperture is its half angle in radians.</summary>
        public Expression CappedTorus(Expression majorRadius, Expression minorRadius, Expression halfAngle) => IqSdf.sdCappedTorus(new Expression3(X, Z, Y), new Expression2(halfAngle.Sin(), halfAngle.Cos()), majorRadius, minorRadius);
        /// <summary>Elongated torus in XY, with the straight center section along Y.</summary>
        public Expression Link(Expression majorRadius, Expression minorRadius, Expression halfLength) => IqSdf.sdLink(Position, halfLength, majorRadius, minorRadius);
        /// <summary>Euclidean distance to a hexagonal prism along Y; radius is the profile inradius.</summary>
        public Expression HexagonalPrism(Expression radius, Expression halfHeight) => IqSdf.sdHexPrism(new Expression3(X, Z, Y), new Expression2(radius, halfHeight));
        /// <summary>Cheaper plane-intersection bound matching HexagonalPrism; radius is the profile inradius.</summary>
        public Expression HexagonalPrismBound(Expression radius, Expression halfHeight) => HexagonPrismField(radius, halfHeight);
        private Expression HexagonPrismField(Expression radius, Expression halfHeight) =>
            ((X.Abs() * (Mathf.Sqrt(3) * 0.5f) + Z.Abs() * 0.5f).Max(Z.Abs()) - radius).Max(Y.Abs() - halfHeight);
        /// <summary>Exact Y-axis triangular prism; radius is the profile circumradius.</summary>
        public Expression TriangularPrism(Expression radius, Expression halfHeight) => ExtrudeProfileY(EquilateralTriangle(radius * (Mathf.Sqrt(3) * 0.5f)), halfHeight);
        /// <summary>Plane-intersection bound for a triangular prism. Profile parameter is its circumradius.</summary>
        public Expression TriangularPrismBound(Expression radius, Expression halfHeight) => IqSdf.sdTriPrism(new Expression3(X, Z, Y), new Expression2(radius, halfHeight));
        /// <summary>Rounded rhombus profile extruded along Y; rounding expands its planar outline.</summary>
        public Expression RhombusPrism(Expression halfWidth, Expression halfDepth, Expression halfHeight, Expression roundingRadius) => IqSdf.sdRhombus(Position, halfWidth, halfDepth, halfHeight, roundingRadius);
        private Expression ExtrudeProfileY(Expression profile, Expression halfHeight) => Remap(ExtrudeZExact(profile, -halfHeight, halfHeight), X, Z, Y);
        /// <summary>Spherical sector pointing toward +Y, with half angle in radians.</summary>
        public Expression SolidAngle(Expression radius, Expression halfAngle)
        {
            var q = new Expression2(Length(new Expression2(X, Z)), Y);
            var direction = new Expression2(halfAngle.Sin(), halfAngle.Cos());
            var projection = q.Dot(direction);
            var side = direction.y * q.x - direction.x * q.y;
            // Use the signed line distance in the interior of the segment,
            // including its endpoints. Multiplying a zero sign by a length
            // discards the cone-face derivative exactly at the apex.
            var interior = projection.GreaterThanOrEqual(0).And(projection.LessThanOrEqual(radius));
            var distance = (q - direction * projection.Clamp(0, radius)).Length();
            return (Position.Length() - radius).Max(interior.Select(side, distance * side.SignNonZero()));
        }
        /// <summary>Sphere portion above Y = cutHeight; abs(cutHeight) must not exceed radius.</summary>
        public Expression CutSphere(Expression radius, Expression cutHeight) => IqSdf.sdCutSphere(Position, radius, cutHeight);
        /// <summary>Open spherical shell below Y = cutHeight; thickness expands both sides of the spherical surface.</summary>
        public Expression CutHollowSphere(Expression radius, Expression cutHeight, Expression thickness) => IqSdf.sdCutHollowSphere(Position, radius, cutHeight, thickness);
        /// <summary>Sphere with a spherical cavity centered on +X at centerDistance.</summary>
        public Expression SphereCavity(Expression radius, Expression cavityRadius, Expression centerDistance) => IqSdf.sdDeathStar(Position, radius, cavityRadius, centerDistance);
        public Expression VesicaSegment(Expression3 start, Expression3 end, Expression halfWidth) => IqSdf.sdVesicaSegment(Position, start, end, halfWidth);
        /// <summary>Euclidean signed distance to an octahedron; extent is the distance from origin to an axial vertex.</summary>
        public Expression Octahedron(Expression extent) => IqSdf.sdOctahedron(Position, extent);
        public Expression OctahedronBound(Expression extent) => IqSdf.sdOctahedronBound(Position, extent);
        /// <summary>Square pyramid with base at Y=-halfHeight and tip at Y=+halfHeight; halfWidth sets its square base half extent.</summary>
        public Expression Pyramid(Expression halfWidth, Expression halfHeight)
        {
            var a = new Expression3(-halfWidth, -halfHeight, -halfWidth);
            var b = new Expression3(halfWidth, -halfHeight, -halfWidth);
            var c = new Expression3(halfWidth, -halfHeight, halfWidth);
            var d = new Expression3(-halfWidth, -halfHeight, halfWidth);
            var tip = new Expression3(0, halfHeight, 0);
            var side = (2 * halfHeight * X.Abs().Max(Z.Abs()) + halfWidth * (Y - halfHeight)) /
                (4 * halfHeight * halfHeight + halfWidth * halfWidth).Sqrt();
            var planes = side.Max(-Y - halfHeight);
            // Interior plane distances give exact signs and finite derivatives on
            // faces. Exterior distances include the base and all four side triangles.
            var surface = QuadSurface(a, b, c, d)
                .Min(TriangleSurface(a, b, tip)).Min(TriangleSurface(b, c, tip))
                .Min(TriangleSurface(c, d, tip)).Min(TriangleSurface(d, a, tip));
            return planes.GreaterThan(0).Select(surface, planes);
        }
        /// <summary>Distance bound, not Euclidean distance. Radii must be positive; the center uses the nearest axial surface distance.</summary>
        public Expression EllipsoidBound(Expression3 radii)
        {
            // IQ's ratio is 0/0 at the origin. Supply the actual center distance
            // so a centered sampling grid cannot introduce a NaN into the mesh.
            return Position.LengthSquared().GreaterThan(0).Select(
                IqSdf.sdEllipsoid(Position, radii), -radii.MinComponent());
        }
        /// <summary>Unsigned distance to an open 3D triangle with non-collinear vertices.</summary>
        public Expression TriangleSurface(Expression3 a, Expression3 b, Expression3 c) => IqSdf.udTriangle(Position, a, b, c);
        /// <summary>Unsigned distance to an open planar convex quad in perimeter order.</summary>
        public Expression QuadSurface(Expression3 a, Expression3 b, Expression3 c, Expression3 d) => IqSdf.udQuad(Position, a, b, c, d);
        public Expression Segment(Expression2 start, Expression2 end) => IqSdf.sdSegment(ProfilePosition, start, end);
        public Expression SegmentSurface(Expression3 start, Expression3 end) => HgSdf.fLineSegment(Position, start, end);
        /// <summary>Unsigned distance to a circle in XY.</summary>
        public Expression CircleCurve(Expression radius) => HgSdf.fCircle(new Expression3(X, Z, Y), radius);
        /// <summary>Unsigned distance to a filled disc in XY.</summary>
        public Expression DiscSurface(Expression radius) => HgSdf.fDisc(new Expression3(X, Z, Y), radius);

        // Exact XY profiles, except where a method explicitly states otherwise.
        public Expression Rhombus(Expression halfWidth, Expression halfHeight) => IqSdf.sdRhombus(ProfilePosition, new Expression2(halfWidth, halfHeight));
        public Expression Trapezoid(Expression lowerHalfWidth, Expression upperHalfWidth, Expression halfHeight) => IqSdf.sdTrapezoid(ProfilePosition, lowerHalfWidth, upperHalfWidth, halfHeight);
        public Expression Parallelogram(Expression halfWidth, Expression halfHeight, Expression skew) => IqSdf.sdParallelogram(ProfilePosition, halfWidth, halfHeight, skew);
        /// <summary>Centered equilateral triangle with base half width, pointing toward +Y.</summary>
        public Expression EquilateralTriangle(Expression halfWidth) => IqSdf.sdEquilateralTriangle(ProfilePosition, halfWidth);
        /// <summary>Isosceles triangle with base half width at Y=-halfHeight and tip at Y=+halfHeight.</summary>
        public Expression IsoscelesTriangle(Expression halfWidth, Expression halfHeight) => IqSdf.sdTriangleIsosceles(new Expression2(X, halfHeight - Y), new Expression2(halfWidth, 2 * halfHeight));
        public Expression Triangle(Expression2 a, Expression2 b, Expression2 c) => IqSdf.sdTriangle(ProfilePosition, a, b, c);
        /// <summary>Uneven capsule with circle centers at Y=+/-halfLength.</summary>
        public Expression UnevenCapsule(Expression lowerRadius, Expression upperRadius, Expression halfLength) => IqSdf.sdUnevenCapsule(new Expression2(X, Y + halfLength), lowerRadius, upperRadius, 2 * halfLength);
        public Expression Pentagon(Expression inradius) => IqSdf.sdPentagon(ProfilePosition, inradius);
        public Expression Hexagon(Expression inradius) => IqSdf.sdHexagon(ProfilePosition, inradius);
        public Expression Octagon(Expression inradius) => IqSdf.sdOctogon(ProfilePosition, inradius);
        public Expression Hexagram(Expression radius) => IqSdf.sdHexagram(ProfilePosition, radius);
        public Expression Pentagram(Expression radius) => IqSdf.sdPentagram(ProfilePosition, radius);
        /// <summary>Regular star; points must be >=3, indentation between 2 and points. Both can be parameters.</summary>
        public Expression Star(Expression radius, Expression points, Expression indentation) => IqSdf.sdStar(ProfilePosition, radius, points, indentation);
        public Expression Pie(Expression radius, Expression halfAngle) => IqSdf.sdPie(ProfilePosition, new Expression2(halfAngle.Sin(), halfAngle.Cos()), radius);
        public Expression CutCircle(Expression radius, Expression cutHeight) => IqSdf.sdCutDisk(ProfilePosition, radius, cutHeight);
        /// <summary>Arc centered toward +Y; tubeRadius rounds its endpoints.</summary>
        public Expression Arc(Expression radius, Expression halfAngle, Expression tubeRadius) => IqSdf.sdArc(ProfilePosition, new Expression2(halfAngle.Sin(), halfAngle.Cos()), radius, tubeRadius);
        /// <summary>Annular sector with flat ends, centered toward +Y; thickness is the full radial thickness.</summary>
        public Expression RingSector(Expression radius, Expression halfAngle, Expression thickness) => IqSdf.sdRing(ProfilePosition, new Expression2(halfAngle.Cos(), halfAngle.Sin()), radius, thickness);
        public Expression Horseshoe(Expression radius, Expression halfAngle, Expression2 armHalfExtents) => IqSdf.sdHorseshoe(ProfilePosition, new Expression2(halfAngle.Cos(), halfAngle.Sin()), radius, armHalfExtents);
        public Expression Vesica(Expression halfWidth, Expression halfHeight) =>
            halfWidth.GreaterThanOrEqual(halfHeight).Select(
                IqSdf.sdVesica(ProfilePosition, halfWidth, halfHeight),
                IqSdf.sdVesica(ProfilePosition.yx, halfHeight, halfWidth));
        public Expression Vesica(Expression2 start, Expression2 end, Expression halfWidth) => IqSdf.sdOrientedVesica(ProfilePosition, start, end, halfWidth);
        public Expression Moon(Expression radius, Expression cutoutRadius, Expression centerDistance) => IqSdf.sdMoon(ProfilePosition, centerDistance, radius, cutoutRadius);
        public Expression RoundedCross(Expression height) => IqSdf.sdRoundedCross(ProfilePosition, height);
        public Expression Egg(Expression centerDistance, Expression lowerRadius, Expression upperRadius, Expression bulge) => IqSdf.sdEgg(ProfilePosition, centerDistance, lowerRadius, upperRadius, bulge);
        public Expression Heart() => IqSdf.sdHeart(ProfilePosition);
        /// <summary>Exact exterior distance, bound interior distance. Positive inset shrinks the outline.</summary>
        public Expression CrossBound(Expression2 halfExtents, Expression inset = default) => IqSdf.sdCross(ProfilePosition, halfExtents, inset);
        /// <summary>Rounded X preserving its outer X/Y half extent; radius must not exceed halfExtent.</summary>
        public Expression RoundedX(Expression halfExtent, Expression radius) => IqSdf.sdRoundedX(ProfilePosition, 2 * (halfExtent - radius), radius);
        public Expression Polygon(params Expression2[] vertices) => IqSdf.sdPolygon(vertices, ProfilePosition);
        public Expression Ellipse(Expression2 radii) => IqSdf.sdEllipse(ProfilePosition, radii);
        /// <summary>Signed distance to the infinite parabola Y=curvature*X^2.</summary>
        public Expression Parabola(Expression curvature) => IqSdf.sdParabola(ProfilePosition, curvature);
        public Expression ParabolaSegment(Expression halfWidth, Expression height) => IqSdf.sdParabola(ProfilePosition, halfWidth, height);
        /// <summary>Unsigned distance to a quadratic Bezier curve; use Round for a stroke.</summary>
        public Expression Bezier(Expression2 start, Expression2 control, Expression2 end) => IqSdf.sdBezier(ProfilePosition, start, control, end);
        public Expression BlobbyCross(Expression height) => IqSdf.sdBlobbyCross(ProfilePosition, height);
        public Expression Tunnel(Expression2 halfExtents) => IqSdf.sdTunnel(ProfilePosition, halfExtents);
        public Expression Stairs(Expression2 stepSize, Expression count) => IqSdf.sdStairs(ProfilePosition, stepSize, count);
        public Expression QuadraticCircle() => IqSdf.sdQuadraticCircle(ProfilePosition);
        public Expression Hyperbola(Expression curvature, Expression halfLength) => IqSdf.sdHyberbola(ProfilePosition, curvature, halfLength);
        public Expression CoolS() => IqSdf.sdfCoolS(ProfilePosition);
        /// <summary>Unsigned distance to a periodic circular wave.</summary>
        public Expression CircleWave(Expression bend, Expression radius) => IqSdf.sdCircleWave(ProfilePosition, bend, radius);
        /// <summary>Signed distance to the negative XY quadrant.</summary>
        public Expression Corner() => HgSdf.fCorner(ProfilePosition);
        /// <summary>Mercury decorative blob field, singular at the origin; not an exact distance.</summary>
        public Expression BlobBound() => HgSdf.fBlob(Position);
        /// <summary>Generalized distance bound over Mercury's normalized direction table, inclusive indices 0..18.</summary>
        public Expression PolyhedronBound(Expression radius, int firstDirection, int lastDirection) => HgSdf.fGDF(Position, radius, firstDirection, lastDirection);
        /// <summary>Power-norm variant; exponent must be positive. Not Euclidean distance.</summary>
        public Expression PolyhedronBound(Expression radius, Expression exponent, int firstDirection, int lastDirection) => HgSdf.fGDF(Position, radius, exponent, firstDirection, lastDirection);
        public Expression OctahedronPowerBound(Expression radius, Expression exponent) => HgSdf.fOctahedron(Position, radius, exponent);
        public Expression DodecahedronBound(Expression radius) => HgSdf.fDodecahedron(Position, radius);
        public Expression DodecahedronBound(Expression radius, Expression exponent) => HgSdf.fDodecahedron(Position, radius, exponent);
        public Expression IcosahedronBound(Expression radius) => HgSdf.fIcosahedron(Position, radius);
        public Expression IcosahedronBound(Expression radius, Expression exponent) => HgSdf.fIcosahedron(Position, radius, exponent);
        public Expression TruncatedOctahedronBound(Expression radius) => HgSdf.fTruncatedOctahedron(Position, radius);
        public Expression TruncatedOctahedronBound(Expression radius, Expression exponent) => HgSdf.fTruncatedOctahedron(Position, radius, exponent);
        public Expression TruncatedIcosahedronBound(Expression radius) => HgSdf.fTruncatedIcosahedron(Position, radius);
        public Expression TruncatedIcosahedronBound(Expression radius, Expression exponent) => HgSdf.fTruncatedIcosahedron(Position, radius, exponent);
    }
}
