using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rounded box offset recipe.</summary>
    public class RoundedBoxOffsetExample : FidgetMesh<RoundedBoxOffsetExample>
    {
        [field: SerializeField, Range(0.1f, 0.3f)] public float Rounding { get; set; } = 0.2f;
        protected override Expression Define(ShapeBuilder<RoundedBoxOffsetExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Rounding);
            return s.Round(s.BoxExact(new Expression3(0.4f)), amount);
        }
    }
}
