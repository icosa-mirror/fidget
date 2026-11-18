using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed polygon recipe.</summary>
    public class PolygonExample : FidgetMesh<PolygonExample>
    {
        [field: SerializeField, Range(0.35f, 0.7f)] public float Width { get; set; } = 0.55f;
        protected override Expression Define(ShapeBuilder<PolygonExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Width);
            return s.ExtrudeZExact(s.Polygon(new Expression2(-amount, -0.4f), new Expression2(amount, -0.4f), new Expression2(0.3f, 0), new Expression2(amount, 0.5f), new Expression2(-amount, 0.5f)), -0.12f, 0.12f);
        }
    }
}
