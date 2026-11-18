using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rounded cylinder recipe.</summary>
    public class RoundedCylinderExample : FidgetMesh<RoundedCylinderExample>
    {
        [field: SerializeField, Range(0.08f, 0.2f)] public float Radius { get; set; } = 0.12f;
        [field: SerializeField, Range(0.25f, 0.95f)] public float OuterRadius { get; set; } = 0.55f;
        [field: SerializeField, Range(0.25f, 0.95f)] public float HalfHeight { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<RoundedCylinderExample> s)
        {
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var outerRadius = s.Parameter(mesh => mesh.OuterRadius);
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.RoundedCylinder(outerRadius, halfHeight, amount);
        }
    }
}
