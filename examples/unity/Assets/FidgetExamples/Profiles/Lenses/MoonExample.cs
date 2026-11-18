using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed moon recipe.</summary>
    public class MoonExample : FidgetMesh<MoonExample>
    {
        [field: SerializeField, Range(0.38f, 0.72f)] public float Separation { get; set; } = 0.6f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        [field: SerializeField, Range(0.48f, 0.74f)] public float Radius { get; set; } = 0.6f;
        [field: SerializeField, Range(0.32f, 0.68f)] public float CutterRadius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<MoonExample> s)
        {
            var cutterRadius = s.Parameter(mesh => mesh.CutterRadius);
            var radius = s.Parameter(mesh => mesh.Radius);
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var amount = s.Parameter(mesh => mesh.Separation);
            return s.ExtrudeZExact(s.Moon(radius, cutterRadius, amount), -halfDepth, halfDepth);
        }
    }
}
