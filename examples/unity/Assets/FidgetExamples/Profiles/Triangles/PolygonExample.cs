using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed polygon recipe.</summary>
    public class PolygonExample : FidgetMesh<PolygonExample>
    {
        [field: SerializeField, Range(0.35f, 0.7f)] public float Width { get; set; } = 0.55f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        [field: SerializeField, Range(-0.18f, 0.33f)] public float NotchX { get; set; } = 0.3f;
        protected override Expression Define(ShapeBuilder<PolygonExample> s)
        {
            var notchX = s.Parameter(mesh => mesh.NotchX);
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var amount = s.Parameter(mesh => mesh.Width);
            return s.ExtrudeZExact(s.Polygon(new Expression2(-amount, -0.4f), new Expression2(amount, -0.4f), new Expression2(notchX, 0), new Expression2(amount, 0.5f), new Expression2(-amount, 0.5f)), -halfDepth, halfDepth);
        }
    }
}
