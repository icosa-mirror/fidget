using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed trapezoid recipe.</summary>
    public class TrapezoidExample : FidgetMesh<TrapezoidExample>
    {
        [field: SerializeField, Range(0.15f, 0.5f)] public float TopWidth { get; set; } = 0.3f;
        protected override Expression Define(ShapeBuilder<TrapezoidExample> s)
        {
            var amount = s.Parameter(mesh => mesh.TopWidth);
            return s.ExtrudeZExact(s.Trapezoid(0.6f, amount, 0.45f), -0.12f, 0.12f);
        }
    }
}
