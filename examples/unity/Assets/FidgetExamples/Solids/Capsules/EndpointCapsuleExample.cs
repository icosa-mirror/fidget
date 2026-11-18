using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed endpoint capsule recipe.</summary>
    public class EndpointCapsuleExample : FidgetMesh<EndpointCapsuleExample>
    {
        [field: SerializeField, Range(0.15f, 0.4f)] public float Radius { get; set; } = 0.25f;
        protected override Expression Define(ShapeBuilder<EndpointCapsuleExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.Capsule(new Expression3(-0.4f, -0.4f, 0), new Expression3(0.4f, 0.4f, 0), amount);
        }
    }
}
