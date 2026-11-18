using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed bezier stroke recipe.</summary>
    public class BezierStrokeExample : FidgetMesh<BezierStrokeExample>
    {
        [field: SerializeField, Range(0.05f, 0.15f)] public float Stroke { get; set; } = 0.09f;
        protected override Expression Define(ShapeBuilder<BezierStrokeExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Stroke);
            return s.ExtrudeZExact(s.Round(s.Bezier(new Expression2(-0.6f, -0.3f), new Expression2(0, 0.7f), new Expression2(0.6f, -0.3f)), amount), -0.12f, 0.12f);
        }
    }
}
