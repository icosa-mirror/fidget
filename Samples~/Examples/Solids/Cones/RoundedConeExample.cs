using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rounded cone recipe.</summary>
    public class RoundedConeExample : FidgetMesh<RoundedConeExample>
    {
        [field: SerializeField, Range(0.25f, 0.65f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<RoundedConeExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.RoundedCone(amount, 0.2f, 0.5f);
        }
    }
}
