using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed hexagonal prism bound recipe.</summary>
    public class HexagonalPrismBoundExample : FidgetMesh<HexagonalPrismBoundExample>
    {
        [field: SerializeField, Range(0.35f, 0.65f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<HexagonalPrismBoundExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.HexagonalPrismBound(amount, 0.4f);
        }
    }
}
