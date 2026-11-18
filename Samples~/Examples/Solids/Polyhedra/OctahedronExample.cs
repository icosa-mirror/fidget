using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed octahedron recipe.</summary>
    public class OctahedronExample : FidgetMesh<OctahedronExample>
    {
        [field: SerializeField, Range(0.25f, 0.65f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<OctahedronExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.Octahedron(amount);
        }
    }
}
