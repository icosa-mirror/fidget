using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed icosahedron recipe.</summary>
    public class IcosahedronExample : FidgetMesh<IcosahedronExample>
    {
        [field: SerializeField, Range(0.25f, 0.6f)] public float Radius { get; set; } = 0.45f;
        protected override Expression Define(ShapeBuilder<IcosahedronExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.IcosahedronBound(amount);
        }
    }
}
