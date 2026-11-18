using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed endpoint capsule recipe.</summary>
    public class EndpointCapsuleExample : FidgetMesh<EndpointCapsuleExample>
    {
        [field: SerializeField, Range(0.07f, 0.52f)] public float Radius { get; set; } = 0.25f;
        [field: SerializeField, Range(0.12f, 0.85f)] public float HalfSpan { get; set; } = 0.4f;
        protected override Expression Define(ShapeBuilder<EndpointCapsuleExample> s)
        {
            var halfSpan = s.Parameter(mesh => mesh.HalfSpan);
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.Capsule(new Expression3(-halfSpan, -0.4f, 0), new Expression3(halfSpan, 0.4f, 0), amount);
        }
    }
}
