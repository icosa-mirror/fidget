using System;
using System.Collections.Generic;
using UnityEngine;

namespace Icosa.Fidget
{
    /// <summary>A scalar expression in a shape graph. Negative values are inside.</summary>
    public readonly struct Expression
    {
        internal readonly ShapeBuilder Owner;
        internal readonly uint Index;
        internal readonly float Constant;
        internal Expression(ShapeBuilder owner, uint index) { Owner = owner; Index = index; Constant = 0; }
        private Expression(float value) { ShapeBuilder.Finite(value, nameof(value)); Owner = null; Index = 0; Constant = value; }
        public static implicit operator Expression(float value) => new Expression(value);

        private static Expression Binary(uint op, Expression a, Expression b)
        {
            var owner = a.Owner ?? b.Owner;
            if (owner != null) return owner.Node(op, a, b);
            switch (op)
            {
                case 11: return a.Constant + b.Constant;
                case 12: return a.Constant - b.Constant;
                case 13: return a.Constant * b.Constant;
                case 14: return a.Constant / b.Constant;
                case 15: return Mathf.Min(a.Constant, b.Constant);
                case 16: return Mathf.Max(a.Constant, b.Constant);
                // Match Fidget's rem_euclid, including negative dividends/divisors.
                case 20: return Mathf.Atan2(a.Constant, b.Constant);
                case 21: return a.Constant < b.Constant ? -1 : a.Constant > b.Constant ? 1 : 0;
                case 22: return a.Constant == 0 ? a : b;
                case 23: return a.Constant != 0 ? a : b;
                default:
                    float remainder = a.Constant % b.Constant;
                    return remainder < 0 ? remainder + Mathf.Abs(b.Constant) : remainder;
            }
        }
        private Expression Unary(uint op)
        {
            if (Owner != null) return Owner.Node(op, this);
            switch (op)
            {
                case 5: return -Constant;
                case 6: return Mathf.Abs(Constant);
                case 7: return Mathf.Sqrt(Constant);
                case 8: return Constant * Constant;
                case 9: return Mathf.Sin(Constant);
                case 10: return Mathf.Cos(Constant);
                case 19: return Mathf.Floor(Constant);
                case 24: return Constant == 0 ? 1 : 0;
                case 25: return Mathf.Exp(Constant);
                default: return Mathf.Log(Constant);
            }
        }
        public static Expression operator +(Expression a, Expression b) => Binary(11, a, b);
        public static Expression operator -(Expression a, Expression b) => Binary(12, a, b);
        public static Expression operator *(Expression a, Expression b) => Binary(13, a, b);
        public static Expression operator /(Expression a, Expression b) => Binary(14, a, b);
        public static Expression operator -(Expression value) => value.Unary(5);
        /// <summary>Clamp a scalar to the inclusive range.</summary>
        public Expression Clamp(Expression lower, Expression upper) => Max(lower).Min(upper);
        public Expression Saturate() => Clamp(0, 1);
        /// <summary>Returns -1 below zero and +1 otherwise, including at zero.</summary>
        public Expression SignNonZero() => LessThan(0).Select(-1, 1);
        public Expression Abs() => Unary(6);
        public Expression Sqrt() => Unary(7);
        public Expression Square() => Unary(8);
        public Expression Sin() => Unary(9);
        public Expression Cos() => Unary(10);
        public Expression Min(Expression other) => Binary(15, this, other);
        public Expression Max(Expression other) => Binary(16, this, other);
        /// <summary>Least non-negative remainder, matching Fidget Euclidean modulo.</summary>
        public Expression Modulo(Expression other) => Binary(18, this, other);
        public Expression Floor() => Unary(19);
        public Expression Atan2(Expression x) => Binary(20, this, x);
        public Expression Compare(Expression other) => Binary(21, this, other);
        public Expression And(Expression other) => Binary(22, this, other);
        public Expression Or(Expression other) => Binary(23, this, other);
        public Expression Not() => Unary(24);
        public Expression Exp() => Unary(25);
        public Expression Ln() => Unary(26);
        public Expression LessThan(Expression other) => other.Compare(this).Max(0);
        public Expression GreaterThan(Expression other) => Compare(other).Max(0);
        public Expression EqualTo(Expression other) => Compare(other).Not();
        public Expression LessThanOrEqual(Expression other) => GreaterThan(other).Not();
        public Expression GreaterThanOrEqual(Expression other) => LessThan(other).Not();
        /// <summary>Choose a value per evaluation point; zero is false. Unselected NaNs do not propagate.</summary>
        public Expression Select(Expression whenTrue, Expression whenFalse) =>
            // Add the masked branches: OR would discard the selected branch's
            // derivative whenever its value is exactly zero (the mesh surface).
            And(whenTrue) + Not().And(whenFalse);
        /// <summary>Power for nonnegative bases and positive exponents (also defines 0^0 = 1).</summary>
        public Expression Pow(Expression exponent)
        {
            if (Owner == null && exponent.Owner == null) return Mathf.Pow(Constant, exponent.Constant);
            var zero = EqualTo(0);
            var safeBase = zero.Select(1, this);
            return zero.Select(exponent.EqualTo(0), (safeBase.Ln() * exponent).Exp());
        }

    }

    /// <summary>A typed parameter key. Resolve it once; set its value without string lookup per frame.</summary>
    public readonly struct Parameter
    {
        internal readonly ShapeBuilder Owner;
        internal readonly int Index;
        private readonly Expression _expression;
        internal Parameter(ShapeBuilder owner, int index, Expression expression) { Owner = owner; Index = index; _expression = expression; }
        public static implicit operator Expression(Parameter parameter)
        {
            if (parameter.Owner == null) throw new ArgumentException("Uninitialized Fidget parameter.");
            return parameter._expression;
        }
    }

    /// <summary>Build once on the main thread, then compile and update parameter values without rebuilding.</summary>
    public partial class ShapeBuilder
    {
        private readonly List<Native.Node> _nodes = new List<Native.Node>();
        private readonly List<float> _defaults = new List<float>();
        private readonly Dictionary<string, Parameter> _parameters = new Dictionary<string, Parameter>();
        public Expression X { get; }
        public Expression Y { get; }
        public Expression Z { get; }
        public ShapeBuilder() { X = Add(new Native.Node { Opcode = 1 }); Y = Add(new Native.Node { Opcode = 2 }); Z = Add(new Native.Node { Opcode = 3 }); }
        internal static void Finite(float value, string name)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) throw new ArgumentOutOfRangeException(name, "Must be finite.");
        }
        private Expression Add(Native.Node node) { _nodes.Add(node); return new Expression(this, (uint)(_nodes.Count - 1)); }
        private uint Import(Expression value)
        {
            if (value.Owner == null) return Add(new Native.Node { Opcode = 0, Value = value.Constant }).Index;
            if (value.Owner != this) throw new ArgumentException("Expressions must belong to the same ShapeBuilder.");
            return value.Index;
        }
        internal Expression Node(uint op, Expression a, Expression b = default) => Add(new Native.Node { Opcode = op, A = Import(a), B = (op >= 11 && op <= 18) || (op >= 20 && op <= 23) ? Import(b) : 0 });
        /// <summary>Low-level unnamed parameter slot. Prefer ShapeBuilder&lt;T&gt; for typed model properties.</summary>
        public Parameter Parameter(float defaultValue = 0)
        {
            Finite(defaultValue, nameof(defaultValue));
            int index = _defaults.Count;
            var key = new Parameter(this, index, Add(new Native.Node { Opcode = 4, A = (uint)index }));
            _defaults.Add(defaultValue);
            return key;
        }
        public Parameter Parameter(string name, float defaultValue)
        {
            if (string.IsNullOrWhiteSpace(name) || _parameters.ContainsKey(name)) throw new ArgumentException("Parameter names must be nonempty and unique.", nameof(name));
            var key = Parameter(defaultValue);
            _parameters.Add(name, key);
            return key;
        }
        public Expression Sphere(Expression radius) => (X.Square() + Y.Square() + Z.Square()).Sqrt() - radius;
        /// <summary>Box dimensions are half extents, in local units.</summary>
        public Expression Box(Expression x, Expression y, Expression z) => (X.Abs() - x).Max(Y.Abs() - y).Max(Z.Abs() - z);
        public Expression Box(Vector3 halfExtents) => Box(halfExtents.x, halfExtents.y, halfExtents.z);
        /// <summary>Capped cylinder along the Y axis.</summary>
        public Expression Cylinder(Expression radius, Expression halfHeight) => ((X.Square() + Z.Square()).Sqrt() - radius).Max(Y.Abs() - halfHeight);
        public Expression Torus(Expression majorRadius, Expression minorRadius) => (((X.Square() + Z.Square()).Sqrt() - majorRadius).Square() + Y.Square()).Sqrt() - minorRadius;
        public Expression Union(Expression a, Expression b) => a.Min(b);
        public Expression Intersect(Expression a, Expression b) => a.Max(b);
        public Expression Subtract(Expression a, Expression b) => a.Max(-b);
        public Expression Remap(Expression shape, Expression x, Expression y, Expression z) => Add(new Native.Node { Opcode = 17, A = Import(shape), B = Import(x), C = Import(y), D = Import(z) });
        public Expression Translate(Expression shape, Expression x, Expression y, Expression z) => Remap(shape, X - x, Y - y, Z - z);
        public Expression Translate(Expression shape, Vector3 offset) => Translate(shape, offset.x, offset.y, offset.z);
        /// <summary>Inverse coordinate mapping rotates the shape around Y, in radians.</summary>
        public Expression RotateY(Expression shape, Expression radians) => Remap(shape, radians.Cos() * X - radians.Sin() * Z, Y, radians.Sin() * X + radians.Cos() * Z);
        /// <summary>Apply a nonsingular affine local-to-model transform.</summary>
        public Expression Transform(Expression shape, Matrix4x4 localToModel)
        {
            for (int i = 0; i < 16; ++i) Finite(localToModel[i], nameof(localToModel));
            if (localToModel.m30 != 0 || localToModel.m31 != 0 || localToModel.m32 != 0 || localToModel.m33 != 1 || Mathf.Abs(localToModel.determinant) < 1e-12f)
                throw new ArgumentException("Transform must be affine and invertible.", nameof(localToModel));
            var m = localToModel.inverse;
            return Remap(shape, m.m00 * X + m.m01 * Y + m.m02 * Z + m.m03, m.m10 * X + m.m11 * Y + m.m12 * Z + m.m13, m.m20 * X + m.m21 * Y + m.m22 * Z + m.m23);
        }
        public unsafe CompiledShape Compile(Expression root)
        {
            Native.CheckAbi();
            // Emit a final identity node so any previously built expression can be the root.
            Node(11, root, 0);
            var nodes = _nodes.ToArray();
            fixed (Native.Node* data = nodes)
            {
                Native.Check(Native.fidget_shape_compile(data, (uint)nodes.Length, (uint)_defaults.Count, out var handle), "compile");
                return new CompiledShape(handle, this, _defaults.ToArray(), new Dictionary<string, Parameter>(_parameters));
            }
        }
    }
}
