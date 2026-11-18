using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed inflated cylinder recipe.</summary>
    public class InflatedCylinderExample : FidgetMesh<InflatedCylinderExample>
    {
        [field: SerializeField, Range(0.1f, 0.3f)] public float Rounding { get; set; } = 0.2f;
        protected override Expression Define(ShapeBuilder<InflatedCylinderExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Rounding);
            return s.Round(s.Cylinder(0.4f, 0.4f), amount);
        }
    }
}
