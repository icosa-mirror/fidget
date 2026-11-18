using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed power icosahedron recipe.</summary>
    public class PowerIcosahedronExample : FidgetMesh<PowerIcosahedronExample>
    {
        [field: SerializeField, Range(3f, 10f)] public float Exponent { get; set; } = 6f;
        protected override Expression Define(ShapeBuilder<PowerIcosahedronExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Exponent);
            return s.IcosahedronBound(0.45f, amount);
        }
    }
}
