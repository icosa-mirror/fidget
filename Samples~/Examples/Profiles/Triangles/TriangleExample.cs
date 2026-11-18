using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed triangle recipe.</summary>
    public class TriangleExample : FidgetMesh<TriangleExample>
    {
        [field: SerializeField, Range(0.35f, 0.7f)] public float Width { get; set; } = 0.55f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        [field: SerializeField, Range(0.1f, 0.96f)] public float TipHeight { get; set; } = 0.6f;
        protected override Expression Define(ShapeBuilder<TriangleExample> s)
        {
            var tipHeight = s.Parameter(mesh => mesh.TipHeight);
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var amount = s.Parameter(mesh => mesh.Width);
            return s.ExtrudeZExact(s.Triangle(new Expression2(-amount, -0.4f), new Expression2(amount, -0.3f), new Expression2(0.2f, tipHeight)), -halfDepth, halfDepth);
        }
    }
}
