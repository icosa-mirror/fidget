using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed stair subtraction recipe.</summary>
    public class StairSubtractionExample : FidgetMesh<StairSubtractionExample>
    {
        [field: SerializeField, Range(2f, 6f)] public int Count { get; set; } = 3;
        protected override Expression Define(ShapeBuilder<StairSubtractionExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Count);
            ExampleInputs.JoinInputs(s, out var a, out var b);
            return s.SubtractStairs(a, b, 0.18f, amount);
        }
    }
}
