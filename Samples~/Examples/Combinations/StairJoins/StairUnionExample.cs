using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed stair union recipe.</summary>
    public class StairUnionExample : FidgetMesh<StairUnionExample>
    {
        [field: SerializeField, Range(2f, 6f)] public int Count { get; set; } = 3;
        protected override Expression Define(ShapeBuilder<StairUnionExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Count);
            ExampleInputs.JoinInputs(s, out var a, out var b);
            return s.UnionStairs(a, b, 0.18f, amount);
        }
    }
}
