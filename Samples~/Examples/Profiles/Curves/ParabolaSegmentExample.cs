using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed parabola segment recipe.</summary>
    public class ParabolaSegmentExample : FidgetMesh<ParabolaSegmentExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<ParabolaSegmentExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.ExtrudeZExact(s.Intersect(s.ParabolaSegment(0.6f, 0.5f), s.RectangleExact(0.65f, 0.6f)), -0.12f, 0.12f), amount);
        }
    }
}
