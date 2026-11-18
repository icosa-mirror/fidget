using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed uneven capsule recipe.</summary>
    public class UnevenCapsuleExample : FidgetMesh<UnevenCapsuleExample>
    {
        [field: SerializeField, Range(0.1f, 0.26f)] public float TipRadius { get; set; } = 0.15f;
        [field: SerializeField, Range(0.1f, 0.38f)] public float BaseRadius { get; set; } = 0.3f;
        [field: SerializeField, Range(0.32f, 0.85f)] public float HalfHeight { get; set; } = 0.4f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<UnevenCapsuleExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var baseRadius = s.Parameter(mesh => mesh.BaseRadius);
            var amount = s.Parameter(mesh => mesh.TipRadius);
            return s.ExtrudeZExact(s.UnevenCapsule(baseRadius, amount, halfHeight), -halfDepth, halfDepth);
        }
    }
}
