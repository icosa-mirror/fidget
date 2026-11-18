using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rounded cone recipe.</summary>
    public class RoundedConeExample : FidgetMesh<RoundedConeExample>
    {
        [field: SerializeField, Range(0.25f, 0.65f)] public float Radius { get; set; } = 0.5f;
        [field: SerializeField, Range(0.06f, 0.58f)] public float UpperRadius { get; set; } = 0.2f;
        [field: SerializeField, Range(0.34f, 0.95f)] public float HalfHeight { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<RoundedConeExample> s)
        {
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var upperRadius = s.Parameter(mesh => mesh.UpperRadius);
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.RoundedCone(amount, upperRadius, halfHeight);
        }
    }
}
