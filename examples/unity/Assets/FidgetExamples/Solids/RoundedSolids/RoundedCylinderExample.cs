using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rounded cylinder recipe.</summary>
    public class RoundedCylinderExample : FidgetMesh<RoundedCylinderExample>
    {
        [field: SerializeField, Range(0.08f, 0.2f)] public float Radius { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<RoundedCylinderExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.RoundedCylinder(0.55f, 0.5f, amount);
        }
    }
}
