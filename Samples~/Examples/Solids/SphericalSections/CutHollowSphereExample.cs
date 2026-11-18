using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed cut hollow sphere recipe.</summary>
    public class CutHollowSphereExample : FidgetMesh<CutHollowSphereExample>
    {
        [field: SerializeField, Range(-0.42f, 0.42f)] public float CutHeight { get; set; } = 0.1f;
        [field: SerializeField, Range(0.48f, 1.0f)] public float Radius { get; set; } = 0.6f;
        [field: SerializeField, Range(0.04f, 0.18f)] public float HalfThickness { get; set; } = 0.09f;
        protected override Expression Define(ShapeBuilder<CutHollowSphereExample> s)
        {
            var halfThickness = s.Parameter(mesh => mesh.HalfThickness);
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.CutHeight);
            return s.CutHollowSphere(radius, amount, halfThickness);
        }
    }
}
