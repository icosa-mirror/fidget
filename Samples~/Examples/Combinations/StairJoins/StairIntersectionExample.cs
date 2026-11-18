using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed stair intersection recipe.</summary>
    public class StairIntersectionExample : FidgetMesh<StairIntersectionExample>
    {
        [field: SerializeField, Range(2f, 6f)] public int Count { get; set; } = 3;
        protected override Expression Define(ShapeBuilder<StairIntersectionExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Count);
            ExampleInputs.JoinInputs(s, out var a, out var b);
            return s.IntersectStairs(a, b, 0.18f, amount);
        }
    }
}
