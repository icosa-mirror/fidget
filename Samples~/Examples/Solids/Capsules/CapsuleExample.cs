using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed capsule recipe.</summary>
    public class CapsuleExample : FidgetMesh<CapsuleExample>
    {
        [field: SerializeField, Range(0.07f, 0.52f)] public float Radius { get; set; } = 0.25f;
        [field: SerializeField, Range(0f, 1f)] public float HalfLength { get; set; } = 0.4f;
        protected override Expression Define(ShapeBuilder<CapsuleExample> s)
        {
            var halfLength = s.Parameter(mesh => mesh.HalfLength);
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.Capsule(amount, halfLength);
        }
    }
}
