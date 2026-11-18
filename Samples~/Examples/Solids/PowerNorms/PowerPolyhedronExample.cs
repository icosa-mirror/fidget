using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed power polyhedron recipe.</summary>
    public class PowerPolyhedronExample : FidgetMesh<PowerPolyhedronExample>
    {
        [field: SerializeField, Range(3f, 10f)] public float Exponent { get; set; } = 6f;
        [field: SerializeField, Range(0.2f, 0.85f)] public float Radius { get; set; } = 0.45f;
        protected override Expression Define(ShapeBuilder<PowerPolyhedronExample> s)
        {
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.Exponent);
            return s.PolyhedronBound(radius, amount, 0, 6);
        }
    }
}
