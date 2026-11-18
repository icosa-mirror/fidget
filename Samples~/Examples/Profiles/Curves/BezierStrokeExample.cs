using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed bezier stroke recipe.</summary>
    public class BezierStrokeExample : FidgetMesh<BezierStrokeExample>
    {
        [field: SerializeField, Range(0.035f, 0.22f)] public float Stroke { get; set; } = 0.09f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        [field: SerializeField, Range(-0.13f, 1.1f)] public float ControlHeight { get; set; } = 0.7f;
        protected override Expression Define(ShapeBuilder<BezierStrokeExample> s)
        {
            var controlHeight = s.Parameter(mesh => mesh.ControlHeight);
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var amount = s.Parameter(mesh => mesh.Stroke);
            return s.ExtrudeZExact(s.Round(s.Bezier(new Expression2(-0.6f, -0.3f), new Expression2(0, controlHeight), new Expression2(0.6f, -0.3f)), amount), -halfDepth, halfDepth);
        }
    }
}
