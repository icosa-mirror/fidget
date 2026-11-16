using System;
using System.Collections.Generic;
using System.Reflection;
using Icosa.Fidget;
using UnityEngine;

// Evaluate the emitted graph independently of the native compiler. This checks
// coordinate mappings and parameter changes at known interior/exterior points.
static class ShapeLibraryChecks
{
    static float Value(Expression e, Vector3 p, float parameter = 0)
    {
        if (e.Owner == null) return e.Constant;
        var nodes = (List<Native.Node>)typeof(ShapeBuilder).GetField("_nodes", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(e.Owner);
        float Eval(uint i, Vector3 at)
        {
            var n = nodes[(int)i];
            if (n.Opcode == 0) return n.Value;
            if (n.Opcode == 1) return at.x;
            if (n.Opcode == 2) return at.y;
            if (n.Opcode == 3) return at.z;
            if (n.Opcode == 4) return parameter;
            if (n.Opcode == 17) return Eval(n.A, new Vector3(Eval(n.B, at), Eval(n.C, at), Eval(n.D, at)));
            float a = Eval(n.A, at);
            if (n.Opcode == 5) return -a;
            if (n.Opcode == 6) return MathF.Abs(a);
            if (n.Opcode == 7) return MathF.Sqrt(a);
            if (n.Opcode == 8) return a*a;
            if (n.Opcode == 9) return MathF.Sin(a);
            if (n.Opcode == 10) return MathF.Cos(a);
            float b = Eval(n.B, at);
            switch (n.Opcode)
            {
                case 11: return a+b;
                case 12: return a-b;
                case 13: return a*b;
                case 14: return a/b;
                case 15: return MathF.Min(a,b);
                case 16: return MathF.Max(a,b);
                case 18: return a - MathF.Floor(a / MathF.Abs(b)) * MathF.Abs(b);
                default: throw new Exception("Unexpected opcode");
            }
        }
        return Eval(e.Index, p);
    }
    static int count;
    static void Near(float actual, float expected)
    {
        if (!float.IsFinite(actual) || MathF.Abs(actual - expected) > 0.0001f)
            throw new Exception($"Check {count}: expected {expected}, got {actual}");
        count++;
    }
    static void Reject(Action action)
    {
        try { action(); } catch (ArgumentException) { count++; return; }
        throw new Exception("Expected invalid argument rejection");
    }
    static void Main()
    {
        var s = new ShapeBuilder();
        var zero = Vector3.zero;
        Near(Value(s.Circle(new Vector2(1,2), 0.5f), new Vector3(1,2,8)), -0.5f);
        Near(Value(s.Sphere(new Vector3(1,2,3), 0.5f), new Vector3(1,2,3)), -0.5f);
        Near(Value(s.Rectangle(new Vector2(-1,-2), new Vector2(1,2)), zero), -1);
        Near(Value(s.Box(new Vector3(-1,-2,-3), new Vector3(1,2,3)), new Vector3(0,0,4)), 1);
        var sphere = s.Sphere(1);
        Near(Value(s.Inverse(sphere), zero), 1);
        Near(Value(s.Difference(sphere, s.Sphere(0.5f)), zero), 0.5f);
        Near(Value(s.Union(sphere, s.Sphere(2), s.Sphere(3)), zero), -3);
        Near(Value(s.Intersection(sphere, s.Sphere(2), s.Sphere(3)), zero), -1);
        Near(Value(s.Blend(sphere, sphere, 0.4f), zero), -1.1f);
        Near(Value(s.Blend(sphere, sphere, 0), zero), -1);
        Near(Value(s.Blend(sphere, sphere, s.Parameter()), zero, 0.8f), -1.2f);
        Near(Value(s.Move(sphere, new Vector3(2,0,0)), new Vector3(2,0,0)), -1);
        Near(Value(s.Scale(sphere, new Vector3(2,3,4)), new Vector3(0,0,4)), 0);
        Near(Value(s.ScaleUniform(sphere, 2), new Vector3(2,0,0)), 0);
        var point = s.Sphere(new Vector3(1,0,0), 0.1f);
        Near(Value(s.ReflectX(point, 2), new Vector3(3,0,0)), -0.1f);
        Near(Value(s.ReflectY(s.Y, 2), new Vector3(0,3,0)), 1);
        Near(Value(s.ReflectZ(s.Z, 2), new Vector3(0,0,3)), 1);
        Near(Value(s.ReflectXY(point), new Vector3(0,1,0)), -0.1f);
        Near(Value(s.RotateZ(point, MathF.PI/2), new Vector3(0,1,0)), -0.1f);
        Near(Value(s.RotateY(point, MathF.PI/2), new Vector3(0,0,-1)), -0.1f);
        Near(Value(s.RotateX(s.Sphere(new Vector3(0,1,0), 0.1f), MathF.PI/2), new Vector3(0,0,1)), -0.1f);
        Near(Value(s.RotateZ(point, MathF.PI, new Vector3(2,0,0)), new Vector3(3,0,0)), -0.1f);
        Near(Value(s.RotateY(point, MathF.PI/2, zero), new Vector3(0,0,-1)), -0.1f);
        Near(Value(s.Rotate(point, new Vector3(0,0,3), s.Parameter()), new Vector3(0,1,0), MathF.PI/2), -0.1f);
        var profile = s.Circle(0.2f);
        Near(Value(s.RevolveY(profile, 0.6f), new Vector3(0,0,0.6f)), -0.2f);
        Near(Value(s.RevolveY(profile, 0.6f), zero), 0.4f);
        Near(Value(s.ExtrudeZ(profile, -1, 1), zero), -0.2f);
        Near(Value(s.ExtrudeZ(s.Z + profile, -1, 1), new Vector3(0,0,0.5f)), -0.2f);
        Near(Value(s.ExtrudeZ(profile, -1, 1), new Vector3(0,0,2)), 1);
        var loft = s.LoftZ(s.Circle(0.2f), s.Circle(0.6f), -1, 1);
        Near(Value(loft, zero), -0.4f);
        Near(Value(loft, new Vector3(0.2f,0,-1)), 0);
        Near(Value(loft, new Vector3(0.6f,0,1)), 0);
        var repeat = s.RepeatX(s.Sphere(0.1f), 0.5f);
        foreach (float x in new[] {-2f,-1,0,1,2}) Near(Value(repeat, new Vector3(x,0,0)), -0.1f);
        Near(Value(s.RepeatX(point, 2, 0.5f), new Vector3(-3,0,0)), -0.1f);
        Near(Value(s.RepeatX(s.Sphere(0.1f), s.Parameter()), new Vector3(-2,0,0), 1), -0.1f);
        Near(Value(((Expression)(-1.25f)).Modulo(1), zero), 0.75f);
        Near(Value(((Expression)(-1.25f)).Modulo(-1), zero), 0.75f);
        Near(Value(s.X.Modulo(-1), new Vector3(-1.25f,0,0)), 0.75f);
        Reject(() => s.Union(Array.Empty<Expression>()));
        Reject(() => s.Intersection(null));
        Reject(() => s.Reflect(sphere, zero));
        Reject(() => s.Rotate(sphere, new Vector3(float.NaN,0,0), 1));
        Reject(() => s.Union(new[] {new ShapeBuilder().Sphere(1)}));
        Reject(() => s.ExtrudeZ(new ShapeBuilder().Circle(1), -1, 1));
        // Also exercise the native graph compiler with the actual emitted nodes.
        using (s.Compile(s.Union(s.ExtrudeZ(profile, -1, 1), repeat, loft))) { }
        Console.WriteLine($"PASS: {count} shape/parameter/ownership checks and native graph compilation");
    }
}
