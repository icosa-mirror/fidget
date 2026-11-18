using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed power dodecahedron recipe.</summary>
    public class PowerDodecahedronExample : FidgetMesh<PowerDodecahedronExample>
    {
        [field: SerializeField, Range(3f, 10f)] public float Exponent { get; set; } = 6f;
        [field: SerializeField, Range(0.2f, 0.85f)] public float Radius { get; set; } = 0.45f;
        protected override Expression Define(ShapeBuilder<PowerDodecahedronExample> s)
        {
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.Exponent);
            return s.DodecahedronBound(radius, amount);
        }
    }
}
