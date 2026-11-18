using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rounded cross recipe.</summary>
    public class RoundedCrossExample : FidgetMesh<RoundedCrossExample>
    {
        [field: SerializeField, Range(0.4f, 0.75f)] public float Curvature { get; set; } = 0.6f;
        protected override Expression Define(ShapeBuilder<RoundedCrossExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Curvature);
            return s.ExtrudeZExact(s.ScaleUniformExact(s.RoundedCross(amount), 0.6f), -0.12f, 0.12f);
        }
    }
}
