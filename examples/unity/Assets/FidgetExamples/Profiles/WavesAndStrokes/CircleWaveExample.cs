using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed circle wave recipe.</summary>
    public class CircleWaveExample : FidgetMesh<CircleWaveExample>
    {
        [field: SerializeField, Range(0.05f, 0.15f)] public float Stroke { get; set; } = 0.09f;
        [field: SerializeField, Range(0.18f, 0.88f)] public float Bend { get; set; } = 0.4f;
        [field: SerializeField, Range(0.18f, 0.52f)] public float Radius { get; set; } = 0.3f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<CircleWaveExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var radius = s.Parameter(mesh => mesh.Radius);
            var bend = s.Parameter(mesh => mesh.Bend);
            var amount = s.Parameter(mesh => mesh.Stroke);
            return s.ExtrudeZExact(s.Intersect(s.Round(s.CircleWave(bend, radius), amount), s.RectangleExact(0.65f, 0.6f)), -halfDepth, halfDepth);
        }
    }
}
