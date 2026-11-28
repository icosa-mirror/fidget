using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed horseshoe recipe.</summary>
    public class HorseshoeExample : FidgetMesh<HorseshoeExample>
    {
        [field: SerializeField, Range(0.35f, 2.8f)] public float Aperture { get; set; } = 1.4f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        [field: SerializeField, Range(0.22f, 0.72f)] public float Radius { get; set; } = 0.45f;
        [field: SerializeField, Range(0.06f, 0.55f)] public float ArmLength { get; set; } = 0.3f;
        [field: SerializeField, Range(0.04f, 0.18f)] public float HalfThickness { get; set; } = 0.1f;
        protected override Expression Define(ShapeBuilder<HorseshoeExample> s)
        {
            var halfThickness = s.Parameter(mesh => mesh.HalfThickness);
            var armLength = s.Parameter(mesh => mesh.ArmLength);
            var radius = s.Parameter(mesh => mesh.Radius);
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var amount = s.Parameter(mesh => mesh.Aperture);
            return s.ExtrudeZExact(s.Horseshoe(radius, amount, new Expression2(armLength, halfThickness)), -halfDepth, halfDepth);
        }
    }
}
