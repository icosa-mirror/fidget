using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed isosceles triangle recipe.</summary>
    public class IsoscelesTriangleExample : FidgetMesh<IsoscelesTriangleExample>
    {
        [field: SerializeField, Range(0.35f, 0.7f)] public float Width { get; set; } = 0.55f;
        protected override Expression Define(ShapeBuilder<IsoscelesTriangleExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Width);
            return s.ExtrudeZExact(s.IsoscelesTriangle(amount, 0.5f), -0.12f, 0.12f);
        }
    }
}
