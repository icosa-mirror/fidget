using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed octahedron bound recipe.</summary>
    public class OctahedronBoundExample : FidgetMesh<OctahedronBoundExample>
    {
        [field: SerializeField, Range(0.12f, 1.05f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<OctahedronBoundExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.OctahedronBound(amount);
        }
    }
}
