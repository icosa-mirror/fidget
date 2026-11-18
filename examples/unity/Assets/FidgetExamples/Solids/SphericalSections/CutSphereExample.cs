using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed cut sphere recipe.</summary>
    public class CutSphereExample : FidgetMesh<CutSphereExample>
    {
        [field: SerializeField, Range(-0.42f, 0.42f)] public float CutHeight { get; set; } = 0.1f;
        [field: SerializeField, Range(0.48f, 1.0f)] public float Radius { get; set; } = 0.6f;
        protected override Expression Define(ShapeBuilder<CutSphereExample> s)
        {
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.CutHeight);
            return s.CutSphere(radius, amount);
        }
    }
}
