using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed arc recipe.</summary>
    public class ArcExample : FidgetMesh<ArcExample>
    {
        [field: SerializeField, Range(0.2f, 3.08f)] public float Aperture { get; set; } = 1.4f;
        [field: SerializeField, Range(0.28f, 0.9f)] public float Radius { get; set; } = 0.5f;
        [field: SerializeField, Range(0.04f, 0.22f)] public float Stroke { get; set; } = 0.12f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<ArcExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var stroke = s.Parameter(mesh => mesh.Stroke);
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.Aperture);
            return s.ExtrudeZExact(s.Arc(radius, amount, stroke), -halfDepth, halfDepth);
        }
    }
}
