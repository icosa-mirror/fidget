using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed pie recipe.</summary>
    public class PieExample : FidgetMesh<PieExample>
    {
        [field: SerializeField, Range(0.2f, 3.08f)] public float Aperture { get; set; } = 1.4f;
        [field: SerializeField, Range(0.48f, 0.96f)] public float Radius { get; set; } = 0.6f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<PieExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.Aperture);
            return s.ExtrudeZExact(s.Pie(radius, amount), -halfDepth, halfDepth);
        }
    }
}
