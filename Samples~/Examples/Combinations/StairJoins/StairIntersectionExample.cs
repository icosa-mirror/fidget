using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed stair intersection recipe.</summary>
    public class StairIntersectionExample : FidgetMesh<StairIntersectionExample>
    {
        [field: SerializeField, Range(1f, 12f)] public int Count { get; set; } = 3;
        [field: SerializeField, Range(0.1f, 0.38f)] public float JoinRadius { get; set; } = 0.18f;
        [field: SerializeField, Range(0.17f, 0.37f)] public float ArmWidth { get; set; } = 0.22f;
        protected override Expression Define(ShapeBuilder<StairIntersectionExample> s)
        {
            var armWidth = s.Parameter(mesh => mesh.ArmWidth);
            var joinRadius = s.Parameter(mesh => mesh.JoinRadius);
            var amount = s.Parameter(mesh => mesh.Count);
            ExampleInputs.JoinInputs(s, armWidth, out var a, out var b);
            return s.IntersectStairs(a, b, joinRadius, amount);
        }
    }
}
