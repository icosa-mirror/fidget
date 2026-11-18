using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed circle wave recipe.</summary>
    public class CircleWaveExample : FidgetMesh<CircleWaveExample>
    {
        [field: SerializeField, Range(0.05f, 0.15f)] public float Stroke { get; set; } = 0.09f;
        protected override Expression Define(ShapeBuilder<CircleWaveExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Stroke);
            return s.ExtrudeZExact(s.Intersect(s.Round(s.CircleWave(0.4f, 0.3f), amount), s.RectangleExact(0.65f, 0.6f)), -0.12f, 0.12f);
        }
    }
}
