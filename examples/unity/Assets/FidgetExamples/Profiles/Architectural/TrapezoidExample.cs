using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed trapezoid recipe.</summary>
    public class TrapezoidExample : FidgetMesh<TrapezoidExample>
    {
        [field: SerializeField, Range(0.08f, 0.85f)] public float TopWidth { get; set; } = 0.3f;
        [field: SerializeField, Range(0.16f, 0.92f)] public float BaseWidth { get; set; } = 0.6f;
        [field: SerializeField, Range(0.1f, 0.85f)] public float HalfHeight { get; set; } = 0.45f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<TrapezoidExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var baseWidth = s.Parameter(mesh => mesh.BaseWidth);
            var amount = s.Parameter(mesh => mesh.TopWidth);
            return s.ExtrudeZExact(s.Trapezoid(baseWidth, amount, halfHeight), -halfDepth, halfDepth);
        }
    }
}
