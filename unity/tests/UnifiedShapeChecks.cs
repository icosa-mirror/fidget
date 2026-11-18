using System;
using System.Linq;
using System.Reflection;
using Icosa.Fidget;
using UnityEngine;

static partial class ShapeLibraryChecks
{
    static void UnifiedChecks()
    {
        var s = new ShapeBuilder();
        var origin = Vector3.zero;
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
        NativeIqMesh(s, s.RoundedBox(new Expression3(0.6f), 0.15f), "builder rounded box");
        NativeIqMesh(s, s.Frustum(0.6f, 0.25f, 0.7f), "builder frustum");
        Reject(() => s.TwistY(new ShapeBuilder().Sphere(1), 1));
        Reject(() => s.ExtrudeZExact(new ShapeBuilder().Circle(1), -1, 1));
        Reject(() => s.Round(new ShapeBuilder().Sphere(1), 0.1f));
        Console.WriteLine("PASS: unified builder distance, orientation, dimensions, subtraction, repetition identifiers, adjustable count and four native mesh checks");
    }
}
