using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed hexagram recipe.</summary>
    public class HexagramExample : FidgetMesh<HexagramExample>
    {
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        [field: SerializeField, Range(0.15f, 0.75f)] public float Radius { get; set; } = 0.4f;
        protected override Expression Define(ShapeBuilder<HexagramExample> s)
        {
            var radius = s.Parameter(mesh => mesh.Radius);
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            return s.ExtrudeZExact(s.Hexagram(radius), -halfDepth, halfDepth);
        }
    }
}
