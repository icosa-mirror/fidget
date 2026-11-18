using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed egg recipe.</summary>
    public class EggExample : FidgetMesh<EggExample>
    {
        [field: SerializeField, Range(0.1f, 0.21f)] public float TipRadius { get; set; } = 0.15f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        [field: SerializeField, Range(0.5f, 1.15f)] public float CenterDistance { get; set; } = 0.8f;
        [field: SerializeField, Range(0.2f, 0.43f)] public float BaseRadius { get; set; } = 0.3f;
        [field: SerializeField, Range(0.35f, 0.85f)] public float Bulge { get; set; } = 0.6f;
        protected override Expression Define(ShapeBuilder<EggExample> s)
        {
            var bulge = s.Parameter(mesh => mesh.Bulge);
            var baseRadius = s.Parameter(mesh => mesh.BaseRadius);
            var centerDistance = s.Parameter(mesh => mesh.CenterDistance);
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var amount = s.Parameter(mesh => mesh.TipRadius);
            return s.ExtrudeZExact(s.Translate(s.Egg(centerDistance, baseRadius, amount, bulge), 0, -0.25f, 0), -halfDepth, halfDepth);
        }
    }
}
