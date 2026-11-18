using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed triangle recipe.</summary>
    public class TriangleExample : FidgetMesh<TriangleExample>
    {
        [field: SerializeField, Range(0.35f, 0.7f)] public float Width { get; set; } = 0.55f;
        protected override Expression Define(ShapeBuilder<TriangleExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Width);
            return s.ExtrudeZExact(s.Triangle(new Expression2(-amount, -0.4f), new Expression2(amount, -0.3f), new Expression2(0.2f, 0.6f)), -0.12f, 0.12f);
        }
    }
}
