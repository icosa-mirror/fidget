using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed stair profile recipe.</summary>
    public class StairProfileExample : FidgetMesh<StairProfileExample>
    {
        [field: SerializeField, Range(0.12f, 0.27f)] public float StepHeight { get; set; } = 0.2f;
        protected override Expression Define(ShapeBuilder<StairProfileExample> s)
        {
            var amount = s.Parameter(mesh => mesh.StepHeight);
            return s.ExtrudeZExact(s.Translate(s.Stairs(new Expression2(0.2f, amount), 4), -0.4f, -0.4f, 0), -0.12f, 0.12f);
        }
    }
}
