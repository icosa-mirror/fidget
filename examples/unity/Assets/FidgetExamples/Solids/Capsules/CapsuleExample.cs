using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed capsule recipe.</summary>
    public class CapsuleExample : FidgetMesh<CapsuleExample>
    {
        [field: SerializeField, Range(0.15f, 0.4f)] public float Radius { get; set; } = 0.25f;
        protected override Expression Define(ShapeBuilder<CapsuleExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.Capsule(amount, 0.4f);
        }
    }
}
