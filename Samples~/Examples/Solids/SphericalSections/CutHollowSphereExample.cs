using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed cut hollow sphere recipe.</summary>
    public class CutHollowSphereExample : FidgetMesh<CutHollowSphereExample>
    {
        [field: SerializeField, Range(-0.15f, 0.35f)] public float CutHeight { get; set; } = 0.1f;
        protected override Expression Define(ShapeBuilder<CutHollowSphereExample> s)
        {
            var amount = s.Parameter(mesh => mesh.CutHeight);
            return s.CutHollowSphere(0.6f, amount, 0.09f);
        }
    }
}
