using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed dodecahedron recipe.</summary>
    public class DodecahedronExample : FidgetMesh<DodecahedronExample>
    {
        [field: SerializeField, Range(0.12f, 1.05f)] public float Radius { get; set; } = 0.45f;
        protected override Expression Define(ShapeBuilder<DodecahedronExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.DodecahedronBound(amount);
        }
    }
}
