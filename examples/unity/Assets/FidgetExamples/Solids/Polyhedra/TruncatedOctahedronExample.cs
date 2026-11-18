using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed truncated octahedron recipe.</summary>
    public class TruncatedOctahedronExample : FidgetMesh<TruncatedOctahedronExample>
    {
        [field: SerializeField, Range(0.25f, 0.6f)] public float Radius { get; set; } = 0.45f;
        protected override Expression Define(ShapeBuilder<TruncatedOctahedronExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.TruncatedOctahedronBound(amount);
        }
    }
}
