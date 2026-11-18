using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed uneven capsule recipe.</summary>
    public class UnevenCapsuleExample : FidgetMesh<UnevenCapsuleExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<UnevenCapsuleExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.ExtrudeZExact(s.UnevenCapsule(0.3f, 0.15f, 0.4f), -0.12f, 0.12f), amount);
        }
    }
}
