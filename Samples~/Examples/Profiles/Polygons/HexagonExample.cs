using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed hexagon recipe.</summary>
    public class HexagonExample : FidgetMesh<HexagonExample>
    {
        [field: SerializeField, Range(0.3f, 0.6f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<HexagonExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.ExtrudeZExact(s.Hexagon(amount), -0.12f, 0.12f);
        }
    }
}
