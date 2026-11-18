using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed parabola segment recipe.</summary>
    public class ParabolaSegmentExample : FidgetMesh<ParabolaSegmentExample>
    {
        [field: SerializeField, Range(0.25f, 0.65f)] public float Height { get; set; } = 0.5f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        [field: SerializeField, Range(0.2f, 0.9f)] public float HalfWidth { get; set; } = 0.6f;
        protected override Expression Define(ShapeBuilder<ParabolaSegmentExample> s)
        {
            var halfWidth = s.Parameter(mesh => mesh.HalfWidth);
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var amount = s.Parameter(mesh => mesh.Height);
            return s.ExtrudeZExact(s.Intersect(s.ParabolaSegment(halfWidth, amount), s.RectangleExact(0.65f, 0.6f)), -halfDepth, halfDepth);
        }
    }
}
