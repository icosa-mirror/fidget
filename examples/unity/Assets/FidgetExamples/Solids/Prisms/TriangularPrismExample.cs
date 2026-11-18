using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed triangular prism recipe.</summary>
    public class TriangularPrismExample : FidgetMesh<TriangularPrismExample>
    {
        [field: SerializeField, Range(0.3f, 0.65f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<TriangularPrismExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.TriangularPrism(amount, 0.4f);
        }
    }
}
