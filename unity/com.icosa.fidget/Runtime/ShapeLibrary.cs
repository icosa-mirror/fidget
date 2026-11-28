using System;
using UnityEngine;

namespace Icosa.Fidget
{
    /// <summary>Standard shapes built from the same expression operations as fidget-shapes.</summary>
    public partial class ShapeBuilder
    {
        /// <summary>Circle in the XY plane; use ExtrudeZ or RevolveY to make a volume.</summary>
        public Expression Circle(Expression radius) => (X.Square() + Y.Square()).Sqrt() - radius;
        public Expression Circle(Vector2 center, Expression radius) => Translate(Circle(radius), center.x, center.y, 0);
        public Expression Sphere(Vector3 center, Expression radius) => Translate(Sphere(radius), center);
        public Expression Rectangle(Vector2 lower, Vector2 upper) =>
            (lower.x - X).Max(X - upper.x).Max(lower.y - Y).Max(Y - upper.y);
        /// <summary>Box specified by its lower and upper corners.</summary>
        public Expression Box(Vector3 lower, Vector3 upper) =>
            (lower.x - X).Max(X - upper.x).Max(lower.y - Y).Max(Y - upper.y).Max(lower.z - Z).Max(Z - upper.z);

        /// <summary>Balanced union of one or more shapes. Empty inputs are rejected.</summary>
        public Expression Union(params Expression[] shapes) => Combine(shapes, true);
        public Expression Intersection(params Expression[] shapes) => Combine(shapes, false);
        private Expression Combine(Expression[] shapes, bool union)
        {
            if (shapes == null || shapes.Length == 0)
                throw new ArgumentException("At least one shape is required.", nameof(shapes));
            return Combine(shapes, 0, shapes.Length, union);
        }
        private Expression Combine(Expression[] shapes, int start, int count, bool union)
        {
            if (count == 1) { Import(shapes[start]); return shapes[start]; }
            int half = count / 2;
            var a = Combine(shapes, start, half, union);
            var b = Combine(shapes, start + half, count - half, union);
            return union ? a.Min(b) : a.Max(b);
        }
        public Expression Inverse(Expression shape) { Import(shape); return -shape; }
        public Expression Difference(Expression shape, Expression cutout) => Subtract(shape, cutout);
        /// <summary>Quadratic smooth union. Radius must stay positive for expression-valued inputs.</summary>
        public Expression Blend(Expression a, Expression b, Expression radius)
        {
            // Match the upstream hard union fallback for non-positive constants.
            if (radius.Owner == null && radius.Constant <= 0) return Union(a, b);
            return a.Min(b) - (radius - (a - b).Abs()).Max(0).Square() / (4 * radius);
        }
        public Expression Move(Expression shape, Expression x, Expression y, Expression z) => Translate(shape, x, y, z);
        public Expression Move(Expression shape, Vector3 offset) => Translate(shape, offset);
        /// <summary>Scale factors must remain nonzero. Field values are not distance-corrected.</summary>
        public Expression Scale(Expression shape, Expression x, Expression y, Expression z) => Remap(shape, X / x, Y / y, Z / z);
        public Expression Scale(Expression shape, Vector3 scale) => Scale(shape, scale.x, scale.y, scale.z);
        public Expression ScaleUniform(Expression shape, Expression scale) => Scale(shape, scale, scale, scale);

        private static Vector3 UnitAxis(Vector3 axis)
        {
            Finite(axis.x, nameof(axis)); Finite(axis.y, nameof(axis)); Finite(axis.z, nameof(axis));
            float length = axis.magnitude;
            if (length < 1e-8f || length > 1e8f)
                throw new ArgumentException("Axis must have a finite, nonzero length between 1e-8 and 1e8.", nameof(axis));
            return axis / length;
        }
        /// <summary>Reflect about dot(normalized normal, position) = offset.</summary>
        public Expression Reflect(Expression shape, Vector3 normal, Expression offset = default)
        {
            var n = UnitAxis(normal);
            var d = 2 * (n.x * X + n.y * Y + n.z * Z - offset);
            return Remap(shape, X - d * n.x, Y - d * n.y, Z - d * n.z);
        }
        public Expression ReflectX(Expression shape, Expression offset = default) => Reflect(shape, Vector3.right, offset);
        public Expression ReflectY(Expression shape, Expression offset = default) => Reflect(shape, Vector3.up, offset);
        public Expression ReflectZ(Expression shape, Expression offset = default) => Reflect(shape, Vector3.forward, offset);
        /// <summary>Reflect about X = Y, with an optional offset along its unit normal.</summary>
        public Expression ReflectXY(Expression shape, Expression offset = default) => Reflect(shape, new Vector3(-1, 1, 0), offset);

        /// <summary>Rotate about an arbitrary fixed axis and center. Angle is in radians.</summary>
        public Expression Rotate(Expression shape, Vector3 axis, Expression radians, Vector3 center = default)
        {
            var n = UnitAxis(axis);
            var x = X - center.x; var y = Y - center.y; var z = Z - center.z;
            var c = radians.Cos(); var s = radians.Sin(); var t = 1 - c;
            // Inverse Rodrigues rotation maps model coordinates into the shape.
            return Remap(shape,
                (c + n.x*n.x*t)*x + (n.x*n.y*t + n.z*s)*y + (n.x*n.z*t - n.y*s)*z + center.x,
                (n.y*n.x*t - n.z*s)*x + (c + n.y*n.y*t)*y + (n.y*n.z*t + n.x*s)*z + center.y,
                (n.z*n.x*t + n.y*s)*x + (n.z*n.y*t - n.x*s)*y + (c + n.z*n.z*t)*z + center.z);
        }
        public Expression RotateX(Expression shape, Expression radians) => Rotate(shape, Vector3.right, radians);
        public Expression RotateX(Expression shape, Expression radians, Vector3 center) => Rotate(shape, Vector3.right, radians, center);
        public Expression RotateY(Expression shape, Expression radians, Vector3 center) => Rotate(shape, Vector3.up, radians, center);
        public Expression RotateZ(Expression shape, Expression radians) => Rotate(shape, Vector3.forward, radians);
        public Expression RotateZ(Expression shape, Expression radians, Vector3 center) => Rotate(shape, Vector3.forward, radians, center);

        /// <summary>Revolve an XY profile about Y. Offset moves the profile radially outward.</summary>
        public Expression RevolveY(Expression shape, Expression offset = default) =>
            Remap(shape, (X.Square() + Z.Square()).Sqrt() - offset, Y, 0);
        /// <summary>Extrude an XY profile between lower and upper Z coordinates.</summary>
        public Expression ExtrudeZ(Expression shape, Expression lower, Expression upper) =>
            Remap(shape, X, Y, 0).Max(lower - Z).Max(Z - upper);
        /// <summary>Interpolate two XY fields between lower and upper Z coordinates; upper must exceed lower.</summary>
        public Expression LoftZ(Expression a, Expression b, Expression lower, Expression upper)
        {
            var ta = Remap(a, X, Y, 0); var tb = Remap(b, X, Y, 0);
            return (((Z - lower) * tb + (upper - Z) * ta) / (upper - lower)).Max(lower - Z).Max(Z - upper);
        }
        /// <summary>Repeat every 2*radius along X. Radius must stay positive; match fields at cell boundaries.</summary>
        public Expression RepeatX(Expression shape, Expression radius, Expression offset = default)
        {
            var r = radius - offset;
            return Remap(shape, (X + r).Modulo(2 * radius) - r, Y, Z);
        }
    }
}
