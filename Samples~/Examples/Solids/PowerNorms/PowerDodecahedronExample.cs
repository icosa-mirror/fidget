using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed power dodecahedron recipe.</summary>
    public class PowerDodecahedronExample : FidgetMesh<PowerDodecahedronExample>
    {
        [field: SerializeField, Range(3f, 10f)] public float Exponent { get; set; } = 6f;
        protected override Expression Define(ShapeBuilder<PowerDodecahedronExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Exponent);
            return s.DodecahedronBound(0.45f, amount);
        }
    }
}
