using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed power octahedron recipe.</summary>
    public class PowerOctahedronExample : FidgetMesh<PowerOctahedronExample>
    {
        [field: SerializeField, Range(3f, 10f)] public float Exponent { get; set; } = 6f;
        protected override Expression Define(ShapeBuilder<PowerOctahedronExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Exponent);
            return s.OctahedronPowerBound(0.55f, amount);
        }
    }
}
