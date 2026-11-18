using System;
using System.Linq;
using System.Reflection;
using Icosa.Fidget;
using UnityEngine;

static partial class ShapeLibraryChecks
{
    static void UnifiedApiCompilation()
    {
        // Exercise every modelling constructor/operation, including optional out identifiers.
        var files = new[] { "ShapePrimitives.cs", "ShapeOperations.cs" };
        var apiNames = files.SelectMany(file => System.Text.RegularExpressions.Regex.Matches(
            System.IO.File.ReadAllText("unity/com.icosa.fidget/Runtime/" + file), @"public Expression (\w+)\(")
            .Select(match => match.Groups[1].Value)).ToHashSet();
        int compiled = 0;
        foreach (var method in typeof(ShapeBuilder).GetMethods().Where(m => apiNames.Contains(m.Name)))
        {
            var s = new ShapeBuilder();
            object Argument(ParameterInfo parameter)
            {
                string name = parameter.Name;
                var type = parameter.ParameterType;
                if (parameter.IsOut) return null;
                if (type == typeof(Expression))
                {
                    if (name == "shape" || name == "a") return s.Sphere(0.7f);
                    if (name == "b" || name == "cutout" || name == "cutter" || name == "guide") return s.Translate(s.Sphere(0.5f), new Vector3(0.4f, 0, 0));
                    return (Expression)(name switch {
                        "firstCell" => -2f, "lastCell" => 2f, "lower" => -0.5f, "upper" => 0.5f,
                        "count" or "points" => 5f, "indentation" => 3f, "exponent" => 4f,
                        "curvature" => 0.6f, "bulge" => 0.6f, "centerDistance" => 1.2f,
                        "roundingRadius" or "thickness" or "halfThickness" or "edgeWidth" or "chamfer" or "inset" => 0.1f,
                        "cutHeight" => 0.2f, "radius" => 0.6f, "halfAngle" => 0.8f,
                        "offset" => 0f, "startRadius" or "lowerRadius" => 0.5f,
                        "endRadius" or "upperRadius" => 0.25f, "scale" => 1.3f,
                        _ => 0.7f });
                }
                if (type == typeof(Expression2)) return name switch {
                    "a" or "start" => new Expression2(-0.7f, -0.2f),
                    "b" or "end" => new Expression2(0.7f, -0.2f),
                    "c" or "control" => new Expression2(0.1f, 0.8f),
                    _ => new Expression2(1, 0.8f) };
                if (type == typeof(Expression3)) return name switch {
                    "a" or "start" => new Expression3(-0.7f, -0.2f, 0),
                    "b" or "end" => new Expression3(0.7f, -0.2f, 0),
                    "c" => new Expression3(0.7f, 0.8f, 0),
                    "d" => new Expression3(-0.7f, 0.8f, 0),
                    _ => new Expression3(1, 0.8f, 0.7f) };
                if (type == typeof(Expression4)) return new Expression4(0.1f);
                if (type == typeof(Expression2[])) return new[] { new Expression2(-1, 0), new Expression2(1, 0), new Expression2(0, 1) };
                if (type == typeof(Vector3)) return name == "lower" ? new Vector3(-1, -1, -1) : name == "upper" ? Vector3.one : Vector3.up;
                if (type == typeof(Vector2)) return name == "lower" ? new Vector2(-1, -1) : new Vector2(1, 1);
                if (type == typeof(int)) return name == "firstDirection" ? 3 : 6;
                throw new Exception("No unified API fixture for " + parameter);
            }
            try
            {
                var args = method.GetParameters().Select(Argument).ToArray();
                using (s.Compile((Expression)method.Invoke(s, args))) { }
                compiled++;
            }
            catch (Exception ex) { throw new Exception("Unified constructor failed: " + method, ex); }
        }
        Console.WriteLine($"PASS: {compiled} unified constructor/operation overloads compiled natively");
    }

    static void UnifiedChecks()
    {
        if (typeof(IqSdf).IsPublic || typeof(HgSdf).IsPublic)
            throw new Exception("Source-specific APIs must remain internal");
        UnifiedApiCompilation();
        var s = new ShapeBuilder();
        var origin = Vector3.zero;
        Near(Value(s.Rhombus(0.5f, 0.4f), new Vector3(0, 0.7f, 0)), 0.3f);
        Near(Value(s.RhombusPrism(0.5f, 0.4f, 0.35f, 0.1f), new Vector3(0, 0, 0.7f)), 0.2f);
        Near(Value(s.HexagonalPrism(1, 0.5f), new Vector3(0, 0, 1)), 0);
        Near(Value(s.HexagonalPrismBound(1, 0.5f), new Vector3(0, 0, 1)), 0);
        Near(Value(s.TriangularPrism(1, 0.5f), new Vector3(0, 0, 1)), 0);
        Near(Value(s.TriangularPrismBound(1, 0.5f), new Vector3(0, 0, 1)), 0);
        Near(Value(s.Pyramid(0.5f, 0.8f), new Vector3(0, 0.8f, 0)), 0);
        Near(Value(s.Pyramid(0.5f, 0.8f), new Vector3(0, -0.8f, 0)), 0);
        Near(Value(s.RoundedX(0.7f, 0.15f), new Vector3(0.7f, 0.55f, 0)), 0);
        Near(Value(s.RingSector(1, MathF.PI / 2, 0.2f), new Vector3(0, 1, 0)), -0.1f);
        if (Value(s.RingSector(1, MathF.PI / 2, 0.2f), new Vector3(0, -1, 0)) <= 0)
            throw new Exception("Ring sector faces the wrong direction");
        Near(Value(new Expression3(s.X, s.Y, s.Z).LengthSquared(), new Vector3(1, 2, 3)), 14);
        Near(Value(new Expression3(1, s.Y, 3).MinComponent(), new Vector3(0, 2, 0)), 1);
        Near(Value(new Expression3(1, s.Y, 3).MaxComponent(), new Vector3(0, 2, 0)), 3);
        Near(Value(s.Y.SignNonZero(), Vector3.zero), 1);
        Near(Value(new Expression3(1, 0, 0).Cross(new Expression3(0, 1, 0)).z, origin), 1);
        // Independent geometric checks for changed conventions, not merely port-to-facade equality.
        Near(Value(s.Box(1, 1, 1), new Vector3(2, 2, 2)), 1);
        Near(Value(s.BoxExact(new Expression3(1)), new Vector3(2, 2, 2)), MathF.Sqrt(3));
        Near(Value(s.RectangleExact(1, 1), new Vector3(2, 2, 0)), MathF.Sqrt(2));
        Near(Value(s.Cylinder(1, 1), new Vector3(2, 2, 0)), 1);
        Near(Value(s.CylinderExact(1, 1), new Vector3(2, 2, 0)), MathF.Sqrt(2));
        Near(Value(s.ExtrudeZExact(s.Circle(1), -1, 1), new Vector3(2, 0, 2)), MathF.Sqrt(2));
        Near(Value(s.RoundedBox(new Expression3(1), 0.2f), new Vector3(1, 0, 0)), 0);
        Near(Value(s.RoundedRectangle(new Expression2(1), 0.2f), new Vector3(1, 0, 0)), 0);
        Near(Value(s.RoundedCylinder(1, 1, 0.2f), new Vector3(1, 0, 0)), 0);
        Near(Value(s.Round(s.BoxExact(new Expression3(1)), 0.2f), new Vector3(1.2f, 0, 0)), 0);
        Near(Value(s.Shell(s.Sphere(1), 0.1f), new Vector3(1, 0, 0)), -0.1f);
        Near(Value(s.ScaleUniformExact(s.Sphere(1), 2), new Vector3(3, 0, 0)), 1);
        Near(Value(s.ScaleUniformExact(s.Sphere(1), -2), new Vector3(3, 0, 0)), 1);
        Near(Value(s.Cone(1, 1), new Vector3(0, 1, 0)), 0);
        Near(Value(s.Cone(1, 1), new Vector3(0, -1, 0)), 0);
        Near(Value(s.Cone(1, 1), new Vector3(0, 2, 0)), 1);
        Near(Value(s.Capsule(0.2f, 0.7f), new Vector3(0, 0.9f, 0)), 0);
        Near(Value(s.Plane(Vector3.up, 0.4f), new Vector3(0, 0.4f, 0)), 0);
        Near(Value(s.IsoscelesTriangle(1, 1), new Vector3(0, 1, 0)), 0);
        Near(Value(s.IsoscelesTriangle(1, 1), new Vector3(0, -1, 0)), 0);
        Near(Value(s.TwistY(s.Y, 2), new Vector3(1, 0.4f, 0.8f)), 0.4f);
        Near(Value(s.BendZ(s.Z, 2), new Vector3(1, 0.4f, 0.8f)), 0.8f);
        Near(Value(s.SubtractBlend(s.Sphere(1), s.Sphere(0.5f), 0), origin), 0.5f);
        Near(Value(s.SubtractRound(s.Sphere(1), s.Sphere(0.5f), 0.1f), origin), 0.5f);
        Near(Value(s.SubtractChamfer(s.Sphere(1), s.Sphere(0.5f), 0.1f), origin), 0.5f);
        var a = s.Translate(s.Sphere(0.2f), new Vector3(0.3f, 0, 0));
        var positive = s.SymmetryX(a);
        Near(Value(positive, new Vector3(-0.3f, 0, 0)), -0.2f);
        var repeat = s.Repeat(a, new Expression3(1), out var cell);
        Near(Value(repeat, new Vector3(2.3f, 0, 0)), -0.2f);
        Near(Value(cell.x, new Vector3(2.3f, 0, 0)), 2);
        var halfSpacing = s.RepeatX(a, 0.5f, 0.1f, out var ix);
        Near(Value(halfSpacing, new Vector3(2.3f, 0, 0)), Value(s.RepeatX(a, 0.5f, 0.1f), new Vector3(2.3f, 0, 0)));
        Near(Value(ix, new Vector3(2.3f, 0, 0)), 2);
        var finite = s.RepeatLimited(s.Sphere(0.2f), new Expression3(1), new Expression3(2), out var finiteCell);
        Near(Value(finiteCell.x, new Vector3(5, 0, 0)), 2);
        Near(Value(finite, new Vector3(5, 0, 0)), 2.8f);
        Near(Value(finiteCell.x, new Vector3(-5, 0, 0)), -2);
        var repeatedY = s.RepeatY(s.Sphere(0.1f), 0.5f, 0, out var iy);
        Near(Value(repeatedY, new Vector3(0, 3, 0)), -0.1f);
        Near(Value(iy, new Vector3(0, 3, 0)), 3);
        var countParameter = s.Parameter(5);
        var star = s.Star(1, countParameter, 2);
        // Changing the number of points must affect an already-constructed graph.
        var probe = new Vector3(0.4f, 0.6f, 0);
        if (MathF.Abs(Value(star, probe, 5) - Value(star, probe, 6)) < 0.001f) throw new Exception("Star count ignored");
        using (s.Compile(star)) { }
        var polar = s.RepeatPolarY(s.Translate(s.Capsule(0.1f, 0.3f), new Vector3(0.7f, 0, 0)), countParameter, out var sector);
        Near(Value(polar, new Vector3(0.7f, 0, 0), 5), -0.1f);
        Near(Value(sector, new Vector3(0.7f, 0, 0), 5), 0);
        NativeIqMesh(s, s.ExtrudeZExact(star, -0.2f, 0.2f), "builder adjustable star");
        NativeIqMesh(s, s.TwistY(s.BoxExact(new Expression3(0.5f, 0.7f, 0.4f)), 0.7f), "builder twisted box");
        NativeIqMesh(s, s.ExtrudeZExact(s.Vesica(0.25f, 0.6f), -0.12f, 0.12f), "builder tall vesica");
        NativeIqMesh(s, s.ExtrudeZExact(s.Vesica(new Expression2(-0.45f, -0.2f), new Expression2(0.45f, 0.2f), 0.25f), -0.12f, 0.12f), "builder oriented vesica");
        NativeIqMesh(s, s.ExtrudeZExact(s.Moon(0.6f, 0.5f, 0.3f), -0.12f, 0.12f), "builder moon");
        NativeIqMesh(s, s.ExtrudeZExact(s.Horseshoe(0.45f, 1.4f, new Expression2(0.3f, 0.1f)), -0.12f, 0.12f), "builder horseshoe");
        NativeIqMesh(s, s.CutSphere(0.6f, 0.1f), "builder cut sphere");
        NativeIqMesh(s, s.CutHollowSphere(0.6f, 0.1f, 0.09f), "builder cut hollow sphere");
        NativeIqMesh(s, s.SphereCavity(0.6f, 0.45f, 0.45f), "builder sphere cavity");
        NativeIqMesh(s, s.SolidAngle(0.65f, 0.8f), "builder solid angle");
        NativeIqMesh(s, s.ExtrudeZExact(s.Intersect(s.ParabolaSegment(0.6f, 0.5f), s.RectangleExact(0.65f, 0.6f)), -0.12f, 0.12f), "builder parabola segment");
        NativeIqMesh(s, s.ExtrudeZExact(s.Intersect(s.Hyperbola(0.12f, 0.7f), s.RectangleExact(0.6f, 0.6f)), -0.12f, 0.12f), "builder hyperbola");
        NativeIqMesh(s, s.HexagonalPrism(0.5f, 0.4f), "builder hexagonal prism");
        NativeIqMesh(s, s.TriangularPrism(0.5f, 0.4f), "builder triangular prism");
        NativeIqMesh(s, s.RhombusPrism(0.5f, 0.4f, 0.35f, 0.1f), "builder rhombus prism");
        NativeIqMesh(s, s.Cylinder(new Expression3(-0.4f, -0.4f, 0), new Expression3(0.4f, 0.4f, 0), 0.25f), "builder endpoint cylinder caps");
        NativeIqMesh(s, s.RoundedBox(new Expression3(0.6f), 0.15f), "builder rounded box");
        NativeIqMesh(s, s.Frustum(0.6f, 0.25f, 0.7f), "builder frustum");
        Reject(() => s.TwistY(new ShapeBuilder().Sphere(1), 1));
        Reject(() => s.ExtrudeZExact(new ShapeBuilder().Circle(1), -1, 1));
        Reject(() => s.Round(new ShapeBuilder().Sphere(1), 0.1f));
        Console.WriteLine("PASS: unified builder distance, orientation, dimensions, subtraction, repetition identifiers, adjustable count and eighteen native mesh checks");
    }
}
