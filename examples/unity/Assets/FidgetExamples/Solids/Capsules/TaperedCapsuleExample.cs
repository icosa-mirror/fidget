using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed tapered capsule recipe.</summary>
    public class TaperedCapsuleExample : FidgetMesh<TaperedCapsuleExample>
    {
        [field: SerializeField, Range(0.15f, 0.4f)] public float Radius { get; set; } = 0.25f;
        protected override Expression Define(ShapeBuilder<TaperedCapsuleExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.RoundedCone(new Expression3(0, -0.45f, 0), new Expression3(0, 0.45f, 0), amount, 0.15f);
        }
    }
}
