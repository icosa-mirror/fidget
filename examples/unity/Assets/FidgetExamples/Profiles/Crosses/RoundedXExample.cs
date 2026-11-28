using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rounded x recipe.</summary>
    public class RoundedXExample : FidgetMesh<RoundedXExample>
    {
        [field: SerializeField, Range(0.06f, 0.2f)] public float Rounding { get; set; } = 0.12f;
        [field: SerializeField, Range(0.2f, 0.98f)] public float Width { get; set; } = 0.6f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<RoundedXExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var width = s.Parameter(mesh => mesh.Width);
            var amount = s.Parameter(mesh => mesh.Rounding);
            return s.ExtrudeZExact(s.RoundedX(width, amount), -halfDepth, halfDepth);
        }
    }
}
