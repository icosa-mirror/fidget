using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed cut sphere recipe.</summary>
    public class CutSphereExample : FidgetMesh<CutSphereExample>
    {
        [field: SerializeField, Range(-0.15f, 0.35f)] public float CutHeight { get; set; } = 0.1f;
        protected override Expression Define(ShapeBuilder<CutSphereExample> s)
        {
            var amount = s.Parameter(mesh => mesh.CutHeight);
            return s.CutSphere(0.6f, amount);
        }
    }
}
