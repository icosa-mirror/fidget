using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed endpoint cylinder recipe.</summary>
    public class EndpointCylinderExample : FidgetMesh<EndpointCylinderExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<EndpointCylinderExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.Cylinder(new Expression3(-0.4f, -0.4f, 0), new Expression3(0.4f, 0.4f, 0), 0.25f), amount);
        }
    }
}
