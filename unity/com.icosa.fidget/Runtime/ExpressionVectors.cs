using UnityEngine;

namespace Icosa.Fidget
{
    /// <summary>2 scalar expressions; component operations build a graph.</summary>
    public struct Expression2
    {
        public Expression x, y;
        public Expression2(Expression x, Expression y) { this.x = x; this.y = y; }
        public Expression2(Expression value) : this(value, value) { }
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
        public static implicit operator Expression3(Vector3 value) => new Expression3(value.x, value.y, value.z);
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
        public static implicit operator Expression4(Vector4 value) => new Expression4(value.x, value.y, value.z, value.w);
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
