using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed uneven capsule recipe.</summary>
    public class UnevenCapsuleExample : FidgetMesh<UnevenCapsuleExample>
    {
        [field: SerializeField, Range(0.1f, 0.26f)] public float TipRadius { get; set; } = 0.15f;
        protected override Expression Define(ShapeBuilder<UnevenCapsuleExample> s)
        {
            var amount = s.Parameter(mesh => mesh.TipRadius);
            return s.ExtrudeZExact(s.UnevenCapsule(0.3f, amount, 0.4f), -0.12f, 0.12f);
        }
    }
}
