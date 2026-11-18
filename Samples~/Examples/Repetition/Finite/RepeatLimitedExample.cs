using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed repeat limited recipe.</summary>
    public class RepeatLimitedExample : FidgetMesh<RepeatLimitedExample>
    {
        [field: SerializeField, Range(0.08f, 0.15f)] public float Radius { get; set; } = 0.11f;
        protected override Expression Define(ShapeBuilder<RepeatLimitedExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            var bead = s.Sphere(amount);
            var limited = s.RepeatLimited(bead, new Expression3(0.4f), new Expression3(1), out var cell);
            return s.Round(limited, cell.x.Modulo(2) * 0.02f);
        }
    }
}
