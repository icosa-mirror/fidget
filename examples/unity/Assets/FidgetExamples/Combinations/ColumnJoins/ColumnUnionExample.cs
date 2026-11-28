using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed column union recipe.</summary>
    public class ColumnUnionExample : FidgetMesh<ColumnUnionExample>
    {
        [field: SerializeField, Range(2f, 12f)] public int Count { get; set; } = 3;
        [field: SerializeField, Range(0.1f, 0.38f)] public float JoinRadius { get; set; } = 0.18f;
        [field: SerializeField, Range(0.17f, 0.37f)] public float ArmWidth { get; set; } = 0.22f;
        protected override Expression Define(ShapeBuilder<ColumnUnionExample> s)
        {
            var armWidth = s.Parameter(mesh => mesh.ArmWidth);
            var joinRadius = s.Parameter(mesh => mesh.JoinRadius);
            var amount = s.Parameter(mesh => mesh.Count);
            ExampleInputs.JoinInputs(s, armWidth, out var a, out var b);
            return s.UnionColumns(a, b, joinRadius, amount);
        }
    }
}
