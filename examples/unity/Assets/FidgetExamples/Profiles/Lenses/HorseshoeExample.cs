using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed horseshoe recipe.</summary>
    public class HorseshoeExample : FidgetMesh<HorseshoeExample>
    {
        [field: SerializeField, Range(0.8f, 1.8f)] public float Aperture { get; set; } = 1.4f;
        protected override Expression Define(ShapeBuilder<HorseshoeExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Aperture);
            return s.ExtrudeZExact(s.Horseshoe(0.45f, amount, new Expression2(0.3f, 0.1f)), -0.12f, 0.12f);
        }
    }
}
