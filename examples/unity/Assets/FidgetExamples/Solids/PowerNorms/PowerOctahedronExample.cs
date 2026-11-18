using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed power octahedron recipe.</summary>
    public class PowerOctahedronExample : FidgetMesh<PowerOctahedronExample>
    {
        [field: SerializeField, Range(3f, 10f)] public float Exponent { get; set; } = 6f;
        [field: SerializeField, Range(0.2f, 0.85f)] public float Radius { get; set; } = 0.55f;
        protected override Expression Define(ShapeBuilder<PowerOctahedronExample> s)
        {
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.Exponent);
            return s.OctahedronPowerBound(radius, amount);
        }
    }
}
