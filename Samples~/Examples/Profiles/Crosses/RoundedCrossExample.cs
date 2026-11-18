using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rounded cross recipe.</summary>
    public class RoundedCrossExample : FidgetMesh<RoundedCrossExample>
    {
        [field: SerializeField, Range(0.4f, 0.75f)] public float Curvature { get; set; } = 0.6f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<RoundedCrossExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var amount = s.Parameter(mesh => mesh.Curvature);
            return s.ExtrudeZExact(s.ScaleUniformExact(s.RoundedCross(amount), 0.6f), -halfDepth, halfDepth);
        }
    }
}
