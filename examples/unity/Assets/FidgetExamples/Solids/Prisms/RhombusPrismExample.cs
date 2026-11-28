using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rhombus prism recipe.</summary>
    public class RhombusPrismExample : FidgetMesh<RhombusPrismExample>
    {
        [field: SerializeField, Range(0.3f, 0.65f)] public float Radius { get; set; } = 0.5f;
        [field: SerializeField, Range(0.15f, 0.85f)] public float HalfLength { get; set; } = 0.4f;
        [field: SerializeField, Range(0.08f, 0.8f)] public float HalfHeight { get; set; } = 0.35f;
        [field: SerializeField, Range(0f, 0.22f)] public float Rounding { get; set; } = 0.1f;
        protected override Expression Define(ShapeBuilder<RhombusPrismExample> s)
        {
            var rounding = s.Parameter(mesh => mesh.Rounding);
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var halfLength = s.Parameter(mesh => mesh.HalfLength);
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.RhombusPrism(amount, halfLength, halfHeight, rounding);
        }
    }
}
