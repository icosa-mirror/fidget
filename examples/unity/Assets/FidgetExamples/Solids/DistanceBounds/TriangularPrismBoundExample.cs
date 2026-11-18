using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed triangular prism bound recipe.</summary>
    public class TriangularPrismBoundExample : FidgetMesh<TriangularPrismBoundExample>
    {
        [field: SerializeField, Range(0.35f, 0.65f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<TriangularPrismBoundExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.TriangularPrismBound(amount, 0.4f);
        }
    }
}
