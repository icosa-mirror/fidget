using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed segment tube recipe.</summary>
    public class SegmentTubeExample : FidgetMesh<SegmentTubeExample>
    {
        [field: SerializeField, Range(0.05f, 0.18f)] public float Thickness { get; set; } = 0.1f;
        [field: SerializeField, Range(0.12f, 0.85f)] public float HalfSpan { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<SegmentTubeExample> s)
        {
            var halfSpan = s.Parameter(mesh => mesh.HalfSpan);
            var amount = s.Parameter(mesh => mesh.Thickness);
            return s.Round(s.SegmentSurface(new Expression3(-halfSpan, -0.4f, 0), new Expression3(halfSpan, 0.4f, 0)), amount);
        }
    }
}
