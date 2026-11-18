using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed ring sector recipe.</summary>
    public class RingSectorExample : FidgetMesh<RingSectorExample>
    {
        [field: SerializeField, Range(0.2f, 3.08f)] public float Aperture { get; set; } = 1.4f;
        [field: SerializeField, Range(0.28f, 0.9f)] public float Radius { get; set; } = 0.5f;
        [field: SerializeField, Range(0.075f, 0.36f)] public float Thickness { get; set; } = 0.2f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<RingSectorExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var thickness = s.Parameter(mesh => mesh.Thickness);
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.Aperture);
            return s.ExtrudeZExact(s.RingSector(radius, amount, thickness), -halfDepth, halfDepth);
        }
    }
}
