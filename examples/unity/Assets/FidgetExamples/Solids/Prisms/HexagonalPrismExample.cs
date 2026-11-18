using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed hexagonal prism recipe.</summary>
    public class HexagonalPrismExample : FidgetMesh<HexagonalPrismExample>
    {
        [field: SerializeField, Range(0.12f, 1.05f)] public float Radius { get; set; } = 0.5f;
        [field: SerializeField, Range(0.08f, 0.95f)] public float HalfHeight { get; set; } = 0.4f;
        protected override Expression Define(ShapeBuilder<HexagonalPrismExample> s)
        {
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.HexagonalPrism(amount, halfHeight);
        }
    }
}
