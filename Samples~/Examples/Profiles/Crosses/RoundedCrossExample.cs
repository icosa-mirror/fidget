using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rounded cross recipe.</summary>
    public class RoundedCrossExample : FidgetMesh<RoundedCrossExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<RoundedCrossExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.ExtrudeZExact(s.ScaleUniformExact(s.RoundedCross(0.6f), 0.6f), -0.12f, 0.12f), amount);
        }
    }
}
