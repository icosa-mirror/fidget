using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed equilateral triangle recipe.</summary>
    public class EquilateralTriangleExample : FidgetMesh<EquilateralTriangleExample>
    {
        [field: SerializeField, Range(0.35f, 0.7f)] public float Width { get; set; } = 0.55f;
        protected override Expression Define(ShapeBuilder<EquilateralTriangleExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Width);
            return s.ExtrudeZExact(s.EquilateralTriangle(amount), -0.12f, 0.12f);
        }
    }
}
