using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed ellipse recipe.</summary>
    public class EllipseExample : FidgetMesh<EllipseExample>
    {
        [field: SerializeField, Range(0.13f, 0.94f)] public float HalfHeight { get; set; } = 0.4f;
        [field: SerializeField, Range(0.18f, 0.96f)] public float HalfWidth { get; set; } = 0.6f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<EllipseExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var halfWidth = s.Parameter(mesh => mesh.HalfWidth);
            var amount = s.Parameter(mesh => mesh.HalfHeight);
            return s.ExtrudeZExact(s.Ellipse(new Expression2(halfWidth, amount)), -halfDepth, halfDepth);
        }
    }
}
