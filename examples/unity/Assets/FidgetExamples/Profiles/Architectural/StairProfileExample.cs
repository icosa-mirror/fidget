using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed stair profile recipe.</summary>
    public class StairProfileExample : FidgetMesh<StairProfileExample>
    {
        [field: SerializeField, Range(0.07f, 0.38f)] public float StepHeight { get; set; } = 0.2f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        [field: SerializeField, Range(0.07f, 0.34f)] public float StepWidth { get; set; } = 0.2f;
        protected override Expression Define(ShapeBuilder<StairProfileExample> s)
        {
            var stepWidth = s.Parameter(mesh => mesh.StepWidth);
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var amount = s.Parameter(mesh => mesh.StepHeight);
            return s.ExtrudeZExact(s.Translate(s.Stairs(new Expression2(stepWidth, amount), 4), -0.4f, -0.4f, 0), -halfDepth, halfDepth);
        }
    }
}
