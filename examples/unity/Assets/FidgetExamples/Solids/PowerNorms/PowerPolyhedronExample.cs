using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed power polyhedron recipe.</summary>
    public class PowerPolyhedronExample : FidgetMesh<PowerPolyhedronExample>
    {
        [field: SerializeField, Range(3f, 10f)] public float Exponent { get; set; } = 6f;
        protected override Expression Define(ShapeBuilder<PowerPolyhedronExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Exponent);
            return s.PolyhedronBound(0.45f, amount, 0, 6);
        }
    }
}
