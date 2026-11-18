using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed endpoint cylinder recipe.</summary>
    public class EndpointCylinderExample : FidgetMesh<EndpointCylinderExample>
    {
        [field: SerializeField, Range(0.15f, 0.35f)] public float Radius { get; set; } = 0.25f;
        [field: SerializeField, Range(0.12f, 0.85f)] public float HalfSpan { get; set; } = 0.4f;
        protected override Expression Define(ShapeBuilder<EndpointCylinderExample> s)
        {
            var halfSpan = s.Parameter(mesh => mesh.HalfSpan);
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.Cylinder(new Expression3(-halfSpan, -0.4f, 0), new Expression3(halfSpan, 0.4f, 0), amount);
        }
    }
}
