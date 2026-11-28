using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed capped torus recipe.</summary>
    public class CappedTorusExample : FidgetMesh<CappedTorusExample>
    {
        [field: SerializeField, Range(0.1f, 0.25f)] public float TubeRadius { get; set; } = 0.16f;
        [field: SerializeField, Range(0.32f, 1f)] public float MajorRadius { get; set; } = 0.5f;
        [field: SerializeField, Range(0.25f, 3.1f)] public float Aperture { get; set; } = 2.2f;
        protected override Expression Define(ShapeBuilder<CappedTorusExample> s)
        {
            var aperture = s.Parameter(mesh => mesh.Aperture);
            var majorRadius = s.Parameter(mesh => mesh.MajorRadius);
            var amount = s.Parameter(mesh => mesh.TubeRadius);
            return s.CappedTorus(majorRadius, amount, aperture);
        }
    }
}
