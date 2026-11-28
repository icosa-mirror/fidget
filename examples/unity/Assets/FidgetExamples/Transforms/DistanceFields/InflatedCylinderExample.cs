using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed inflated cylinder recipe.</summary>
    public class InflatedCylinderExample : FidgetMesh<InflatedCylinderExample>
    {
        [field: SerializeField, Range(0f, 0.52f)] public float Rounding { get; set; } = 0.2f;
        [field: SerializeField, Range(0.14f, 0.8f)] public float Radius { get; set; } = 0.4f;
        [field: SerializeField, Range(0.12f, 0.8f)] public float HalfHeight { get; set; } = 0.4f;
        protected override Expression Define(ShapeBuilder<InflatedCylinderExample> s)
        {
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.Rounding);
            return s.Round(s.Cylinder(radius, halfHeight), amount);
        }
    }
}
