using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed truncated icosahedron recipe.</summary>
    public class TruncatedIcosahedronExample : FidgetMesh<TruncatedIcosahedronExample>
    {
        [field: SerializeField, Range(0.25f, 0.6f)] public float Radius { get; set; } = 0.45f;
        protected override Expression Define(ShapeBuilder<TruncatedIcosahedronExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.TruncatedIcosahedronBound(amount);
        }
    }
}
