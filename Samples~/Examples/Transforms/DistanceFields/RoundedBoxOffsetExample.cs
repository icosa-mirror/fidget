using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rounded box offset recipe.</summary>
    public class RoundedBoxOffsetExample : FidgetMesh<RoundedBoxOffsetExample>
    {
        [field: SerializeField, Range(0f, 0.52f)] public float Rounding { get; set; } = 0.2f;
        [field: SerializeField, Range(0.12f, 0.78f)] public float HalfWidth { get; set; } = 0.4f;
        [field: SerializeField, Range(0.12f, 0.78f)] public float HalfHeight { get; set; } = 0.4f;
        [field: SerializeField, Range(0.12f, 0.78f)] public float HalfDepth { get; set; } = 0.4f;
        protected override Expression Define(ShapeBuilder<RoundedBoxOffsetExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var halfWidth = s.Parameter(mesh => mesh.HalfWidth);
            var amount = s.Parameter(mesh => mesh.Rounding);
            return s.Round(s.BoxExact(new Expression3(halfWidth, halfHeight, halfDepth)), amount);
        }
    }
}
