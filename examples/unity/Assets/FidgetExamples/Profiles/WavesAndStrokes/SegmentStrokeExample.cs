using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed segment stroke recipe.</summary>
    public class SegmentStrokeExample : FidgetMesh<SegmentStrokeExample>
    {
        [field: SerializeField, Range(0.05f, 0.15f)] public float Stroke { get; set; } = 0.09f;
        protected override Expression Define(ShapeBuilder<SegmentStrokeExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Stroke);
            return s.ExtrudeZExact(s.Round(s.Segment(new Expression2(-0.5f, -0.4f), new Expression2(0.5f, 0.4f)), amount), -0.12f, 0.12f);
        }
    }
}
