using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed dodecahedron recipe.</summary>
    public class DodecahedronExample : FidgetMesh<DodecahedronExample>
    {
        [field: SerializeField, Range(0.25f, 0.6f)] public float Radius { get; set; } = 0.45f;
        protected override Expression Define(ShapeBuilder<DodecahedronExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.DodecahedronBound(amount);
        }
    }
}
