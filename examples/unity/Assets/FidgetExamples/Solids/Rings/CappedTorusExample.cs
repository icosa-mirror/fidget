using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed capped torus recipe.</summary>
    public class CappedTorusExample : FidgetMesh<CappedTorusExample>
    {
        [field: SerializeField, Range(0.1f, 0.25f)] public float TubeRadius { get; set; } = 0.16f;
        protected override Expression Define(ShapeBuilder<CappedTorusExample> s)
        {
            var amount = s.Parameter(mesh => mesh.TubeRadius);
            return s.CappedTorus(0.5f, amount, 2.2f);
        }
    }
}
