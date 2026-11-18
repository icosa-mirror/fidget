using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed isosceles triangle recipe.</summary>
    public class IsoscelesTriangleExample : FidgetMesh<IsoscelesTriangleExample>
    {
        [field: SerializeField, Range(0.35f, 0.7f)] public float Width { get; set; } = 0.55f;
        [field: SerializeField, Range(0.28f, 0.94f)] public float HalfHeight { get; set; } = 0.5f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<IsoscelesTriangleExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var amount = s.Parameter(mesh => mesh.Width);
            return s.ExtrudeZExact(s.IsoscelesTriangle(amount, halfHeight), -halfDepth, halfDepth);
        }
    }
}
