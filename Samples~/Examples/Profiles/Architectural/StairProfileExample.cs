using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed stair profile recipe.</summary>
    public class StairProfileExample : FidgetMesh<StairProfileExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<StairProfileExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.ExtrudeZExact(s.Translate(s.Stairs(new Expression2(0.2f, 0.2f), 4), -0.4f, -0.4f, 0), -0.12f, 0.12f), amount);
        }
    }
}
