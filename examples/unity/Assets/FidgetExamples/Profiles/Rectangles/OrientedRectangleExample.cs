using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed oriented rectangle recipe.</summary>
    public class OrientedRectangleExample : FidgetMesh<OrientedRectangleExample>
    {
        [field: SerializeField, Range(0.1f, 0.28f)] public float HalfWidth { get; set; } = 0.2f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        [field: SerializeField, Range(0.12f, 0.85f)] public float HalfSpan { get; set; } = 0.4f;
        protected override Expression Define(ShapeBuilder<OrientedRectangleExample> s)
        {
            var halfSpan = s.Parameter(mesh => mesh.HalfSpan);
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var amount = s.Parameter(mesh => mesh.HalfWidth);
            return s.ExtrudeZExact(s.OrientedRectangle(new Expression2(-halfSpan, -0.3f), new Expression2(halfSpan, 0.3f), amount), -halfDepth, halfDepth);
        }
    }
}
