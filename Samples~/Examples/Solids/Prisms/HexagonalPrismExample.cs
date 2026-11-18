using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed hexagonal prism recipe.</summary>
    public class HexagonalPrismExample : FidgetMesh<HexagonalPrismExample>
    {
        [field: SerializeField, Range(0.3f, 0.65f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<HexagonalPrismExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.HexagonalPrism(amount, 0.4f);
        }
    }
}
