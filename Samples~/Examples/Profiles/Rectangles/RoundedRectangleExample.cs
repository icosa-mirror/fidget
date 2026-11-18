using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rounded rectangle recipe.</summary>
    public class RoundedRectangleExample : FidgetMesh<RoundedRectangleExample>
    {
        [field: SerializeField, Range(0.08f, 0.25f)] public float Rounding { get; set; } = 0.15f;
        protected override Expression Define(ShapeBuilder<RoundedRectangleExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Rounding);
            return s.ExtrudeZExact(s.RoundedRectangle(new Expression2(0.6f, 0.45f), new Expression4(amount, 0.05f, 0.2f, 0.1f)), -0.12f, 0.12f);
        }
    }
}
