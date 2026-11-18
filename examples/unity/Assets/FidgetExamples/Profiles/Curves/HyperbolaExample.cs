using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed hyperbola recipe.</summary>
    public class HyperbolaExample : FidgetMesh<HyperbolaExample>
    {
        [field: SerializeField, Range(0.06f, 0.18f)] public float Curvature { get; set; } = 0.12f;
        [field: SerializeField, Range(0.035f, 0.22f)] public float Stroke { get; set; } = 0.045f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<HyperbolaExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var stroke = s.Parameter(mesh => mesh.Stroke);
            var amount = s.Parameter(mesh => mesh.Curvature);
            var curve = s.Shell(s.Hyperbola(amount, 0.7f), stroke);
            var window = s.RectangleExact(0.85f, 0.85f);
            return s.ExtrudeZExact(s.Intersect(curve, window), -halfDepth, halfDepth);
        }
    }
}
