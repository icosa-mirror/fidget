using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed segment stroke recipe.</summary>
    public class SegmentStrokeExample : FidgetMesh<SegmentStrokeExample>
    {
        [field: SerializeField, Range(0.05f, 0.15f)] public float Stroke { get; set; } = 0.09f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        [field: SerializeField, Range(0.12f, 0.85f)] public float HalfSpan { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<SegmentStrokeExample> s)
        {
            var halfSpan = s.Parameter(mesh => mesh.HalfSpan);
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var amount = s.Parameter(mesh => mesh.Stroke);
            return s.ExtrudeZExact(s.Round(s.Segment(new Expression2(-halfSpan, -0.4f), new Expression2(halfSpan, 0.4f)), amount), -halfDepth, halfDepth);
        }
    }
}
