using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed chamfered rectangle recipe.</summary>
    public class ChamferedRectangleExample : FidgetMesh<ChamferedRectangleExample>
    {
        [field: SerializeField, Range(0.08f, 0.25f)] public float Rounding { get; set; } = 0.15f;
        [field: SerializeField, Range(0.3f, 0.96f)] public float HalfWidth { get; set; } = 0.6f;
        [field: SerializeField, Range(0.3f, 0.85f)] public float HalfHeight { get; set; } = 0.45f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<ChamferedRectangleExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var halfWidth = s.Parameter(mesh => mesh.HalfWidth);
            var amount = s.Parameter(mesh => mesh.Rounding);
            return s.ExtrudeZExact(s.ChamferedRectangle(new Expression2(halfWidth, halfHeight), amount), -halfDepth, halfDepth);
        }
    }
}
