using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed pentagram recipe.</summary>
    public class PentagramExample : FidgetMesh<PentagramExample>
    {
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        [field: SerializeField, Range(0.16f, 0.88f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<PentagramExample> s)
        {
            var radius = s.Parameter(mesh => mesh.Radius);
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            return s.ExtrudeZExact(s.Pentagram(radius), -halfDepth, halfDepth);
        }
    }
}
