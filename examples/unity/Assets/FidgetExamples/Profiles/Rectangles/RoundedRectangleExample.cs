using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rounded rectangle recipe.</summary>
    public class RoundedRectangleExample : FidgetMesh<RoundedRectangleExample>
    {
        [field: SerializeField, Range(0.08f, 0.25f)] public float Rounding { get; set; } = 0.15f;
        [field: SerializeField, Range(0.3f, 0.96f)] public float HalfWidth { get; set; } = 0.6f;
        [field: SerializeField, Range(0.3f, 0.85f)] public float HalfHeight { get; set; } = 0.45f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<RoundedRectangleExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var halfWidth = s.Parameter(mesh => mesh.HalfWidth);
            var amount = s.Parameter(mesh => mesh.Rounding);
            return s.ExtrudeZExact(s.RoundedRectangle(new Expression2(halfWidth, halfHeight), new Expression4(amount, 0.05f, 0.2f, 0.1f)), -halfDepth, halfDepth);
        }
    }
}
