using UnityEngine;

namespace Icosa.Fidget
{
    /// <summary>2 scalar expressions; component operations build a graph.</summary>
    public struct Expression2
    {
        public Expression x, y;
        public Expression2(Expression x, Expression y) { this.x = x; this.y = y; }
        public Expression2(Expression value) : this(value, value) { }
        /// <summary>Component-wise absolute value.</summary>
        public Expression2 Abs() => new Expression2(x.Abs(), y.Abs());
        public Expression2 Floor() => new Expression2(x.Floor(), y.Floor());
        public Expression2 SignNonZero() => new Expression2(x.SignNonZero(), y.SignNonZero());
        public Expression2 Min(Expression2 other) => new Expression2(x.Min(other.x), y.Min(other.y));
        public Expression2 Max(Expression2 other) => new Expression2(x.Max(other.x), y.Max(other.y));
        public Expression MinComponent() => x.Min(y);
        public Expression MaxComponent() => x.Max(y);
        public Expression Dot(Expression2 other) => x * other.x + y * other.y;
        public Expression LengthSquared() => Dot(this);
        public Expression Length() => LengthSquared().Sqrt();
        /// <summary>Power norm for positive exponent; only exponent 2 gives Euclidean length.</summary>
        public Expression LengthPower(Expression exponent) => (x.Abs().Pow(exponent) + y.Abs().Pow(exponent)).Pow(1 / exponent);
        public static implicit operator Expression2(Vector2 value) => new Expression2(value.x, value.y);
        public Expression2 xy { get => this; set { x = value.x; y = value.y; } }
        public Expression2 yx { get => new Expression2(y, x); set { y = value.x; x = value.y; } }
        public static Expression2 operator -(Expression2 a) => new Expression2(-a.x, -a.y);
        public static Expression2 operator +(Expression2 a, Expression2 b) => new Expression2(a.x + b.x, a.y + b.y);
        public static Expression2 operator +(Expression2 a, Expression b) => new Expression2(a.x + b, a.y + b);
        public static Expression2 operator +(Expression a, Expression2 b) => new Expression2(a + b.x, a + b.y);
        public static Expression2 operator -(Expression2 a, Expression2 b) => new Expression2(a.x - b.x, a.y - b.y);
        public static Expression2 operator -(Expression2 a, Expression b) => new Expression2(a.x - b, a.y - b);
        public static Expression2 operator -(Expression a, Expression2 b) => new Expression2(a - b.x, a - b.y);
        public static Expression2 operator *(Expression2 a, Expression2 b) => new Expression2(a.x * b.x, a.y * b.y);
        public static Expression2 operator *(Expression2 a, Expression b) => new Expression2(a.x * b, a.y * b);
        public static Expression2 operator *(Expression a, Expression2 b) => new Expression2(a * b.x, a * b.y);
        public static Expression2 operator /(Expression2 a, Expression2 b) => new Expression2(a.x / b.x, a.y / b.y);
        public static Expression2 operator /(Expression2 a, Expression b) => new Expression2(a.x / b, a.y / b);
        public static Expression2 operator /(Expression a, Expression2 b) => new Expression2(a / b.x, a / b.y);
    }

    /// <summary>3 scalar expressions; component operations build a graph.</summary>
    public struct Expression3
    {
        public Expression x, y, z;
        public Expression3(Expression x, Expression y, Expression z) { this.x = x; this.y = y; this.z = z; }
        public Expression3(Expression value) : this(value, value, value) { }
        /// <summary>Component-wise absolute value.</summary>
        public Expression3 Abs() => new Expression3(x.Abs(), y.Abs(), z.Abs());
        public Expression3 Floor() => new Expression3(x.Floor(), y.Floor(), z.Floor());
        public Expression3 SignNonZero() => new Expression3(x.SignNonZero(), y.SignNonZero(), z.SignNonZero());
        public Expression3 Min(Expression3 other) => new Expression3(x.Min(other.x), y.Min(other.y), z.Min(other.z));
        public Expression3 Max(Expression3 other) => new Expression3(x.Max(other.x), y.Max(other.y), z.Max(other.z));
        public Expression MinComponent() => x.Min(y.Min(z));
        public Expression MaxComponent() => x.Max(y.Max(z));
        public Expression Dot(Expression3 other) => x * other.x + y * other.y + z * other.z;
        public Expression LengthSquared() => Dot(this);
        public Expression Length() => LengthSquared().Sqrt();
        /// <summary>Power norm for positive exponent; only exponent 2 gives Euclidean length.</summary>
        public Expression LengthPower(Expression exponent) => (x.Abs().Pow(exponent) + y.Abs().Pow(exponent) + z.Abs().Pow(exponent)).Pow(1 / exponent);
        public Expression3 Cross(Expression3 other) => new Expression3(y * other.z - z * other.y, z * other.x - x * other.z, x * other.y - y * other.x);
        public static implicit operator Expression3(Vector3 value) => new Expression3(value.x, value.y, value.z);
        public Expression3 xyz { get => this; set { x = value.x; y = value.y; z = value.z; } }
        public Expression3 zxy { get => new Expression3(z, x, y); set { z = value.x; x = value.y; y = value.z; } }
        public Expression2 xy { get => new Expression2(x, y); set { x = value.x; y = value.y; } }
        public Expression2 xz { get => new Expression2(x, z); set { x = value.x; z = value.y; } }
        public Expression2 zx { get => new Expression2(z, x); set { z = value.x; x = value.y; } }
        public Expression2 yx { get => new Expression2(y, x); set { y = value.x; x = value.y; } }
        public Expression2 yz { get => new Expression2(y, z); set { y = value.x; z = value.y; } }
        public Expression2 zy { get => new Expression2(z, y); set { z = value.x; y = value.y; } }
        public Expression3 yzx { get => new Expression3(y, z, x); set { y = value.x; z = value.y; x = value.z; } }
        public Expression3(Expression2 xy, Expression z) : this(xy.x, xy.y, z) { }
        public Expression3(Expression x, Expression2 yz) : this(x, yz.x, yz.y) { }
        public static Expression3 operator -(Expression3 a) => new Expression3(-a.x, -a.y, -a.z);
        public static Expression3 operator +(Expression3 a, Expression3 b) => new Expression3(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Expression3 operator +(Expression3 a, Expression b) => new Expression3(a.x + b, a.y + b, a.z + b);
        public static Expression3 operator +(Expression a, Expression3 b) => new Expression3(a + b.x, a + b.y, a + b.z);
        public static Expression3 operator -(Expression3 a, Expression3 b) => new Expression3(a.x - b.x, a.y - b.y, a.z - b.z);
        public static Expression3 operator -(Expression3 a, Expression b) => new Expression3(a.x - b, a.y - b, a.z - b);
        public static Expression3 operator -(Expression a, Expression3 b) => new Expression3(a - b.x, a - b.y, a - b.z);
        public static Expression3 operator *(Expression3 a, Expression3 b) => new Expression3(a.x * b.x, a.y * b.y, a.z * b.z);
        public static Expression3 operator *(Expression3 a, Expression b) => new Expression3(a.x * b, a.y * b, a.z * b);
        public static Expression3 operator *(Expression a, Expression3 b) => new Expression3(a * b.x, a * b.y, a * b.z);
        public static Expression3 operator /(Expression3 a, Expression3 b) => new Expression3(a.x / b.x, a.y / b.y, a.z / b.z);
        public static Expression3 operator /(Expression3 a, Expression b) => new Expression3(a.x / b, a.y / b, a.z / b);
        public static Expression3 operator /(Expression a, Expression3 b) => new Expression3(a / b.x, a / b.y, a / b.z);
    }

    /// <summary>4 scalar expressions; component operations build a graph.</summary>
    public struct Expression4
    {
        public Expression x, y, z, w;
        public Expression4(Expression x, Expression y, Expression z, Expression w) { this.x = x; this.y = y; this.z = z; this.w = w; }
        public Expression4(Expression value) : this(value, value, value, value) { }
        /// <summary>Component-wise absolute value.</summary>
        public Expression4 Abs() => new Expression4(x.Abs(), y.Abs(), z.Abs(), w.Abs());
        public Expression4 Floor() => new Expression4(x.Floor(), y.Floor(), z.Floor(), w.Floor());
        public Expression4 SignNonZero() => new Expression4(x.SignNonZero(), y.SignNonZero(), z.SignNonZero(), w.SignNonZero());
        public Expression4 Min(Expression4 other) => new Expression4(x.Min(other.x), y.Min(other.y), z.Min(other.z), w.Min(other.w));
        public Expression4 Max(Expression4 other) => new Expression4(x.Max(other.x), y.Max(other.y), z.Max(other.z), w.Max(other.w));
        public Expression MinComponent() => x.Min(y.Min(z.Min(w)));
        public Expression MaxComponent() => x.Max(y.Max(z.Max(w)));
        public Expression Dot(Expression4 other) => x * other.x + y * other.y + z * other.z + w * other.w;
        public Expression LengthSquared() => Dot(this);
        public Expression Length() => LengthSquared().Sqrt();
        /// <summary>Power norm for positive exponent; only exponent 2 gives Euclidean length.</summary>
        public Expression LengthPower(Expression exponent) => (x.Abs().Pow(exponent) + y.Abs().Pow(exponent) + z.Abs().Pow(exponent) + w.Abs().Pow(exponent)).Pow(1 / exponent);
        public static implicit operator Expression4(Vector4 value) => new Expression4(value.x, value.y, value.z, value.w);
        public Expression2 xy { get => new Expression2(x, y); set { x = value.x; y = value.y; } }
        public Expression2 yx { get => new Expression2(y, x); set { y = value.x; x = value.y; } }
        public Expression2 zw { get => new Expression2(z, w); set { z = value.x; w = value.y; } }
        public static Expression4 operator -(Expression4 a) => new Expression4(-a.x, -a.y, -a.z, -a.w);
        public static Expression4 operator +(Expression4 a, Expression4 b) => new Expression4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        public static Expression4 operator +(Expression4 a, Expression b) => new Expression4(a.x + b, a.y + b, a.z + b, a.w + b);
        public static Expression4 operator +(Expression a, Expression4 b) => new Expression4(a + b.x, a + b.y, a + b.z, a + b.w);
        public static Expression4 operator -(Expression4 a, Expression4 b) => new Expression4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        public static Expression4 operator -(Expression4 a, Expression b) => new Expression4(a.x - b, a.y - b, a.z - b, a.w - b);
        public static Expression4 operator -(Expression a, Expression4 b) => new Expression4(a - b.x, a - b.y, a - b.z, a - b.w);
        public static Expression4 operator *(Expression4 a, Expression4 b) => new Expression4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        public static Expression4 operator *(Expression4 a, Expression b) => new Expression4(a.x * b, a.y * b, a.z * b, a.w * b);
        public static Expression4 operator *(Expression a, Expression4 b) => new Expression4(a * b.x, a * b.y, a * b.z, a * b.w);
        public static Expression4 operator /(Expression4 a, Expression4 b) => new Expression4(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
        public static Expression4 operator /(Expression4 a, Expression b) => new Expression4(a.x / b, a.y / b, a.z / b, a.w / b);
        public static Expression4 operator /(Expression a, Expression4 b) => new Expression4(a / b.x, a / b.y, a / b.z, a / b.w);
    }

}
