using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed parabola segment recipe.</summary>
    public class ParabolaSegmentExample : FidgetMesh<ParabolaSegmentExample>
    {
        [field: SerializeField, Range(0.25f, 0.65f)] public float Height { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<ParabolaSegmentExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Height);
            return s.ExtrudeZExact(s.Intersect(s.ParabolaSegment(0.6f, amount), s.RectangleExact(0.65f, 0.6f)), -0.12f, 0.12f);
        }
    }
}
