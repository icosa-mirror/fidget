using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed column union recipe.</summary>
    public class ColumnUnionExample : FidgetMesh<ColumnUnionExample>
    {
        [field: SerializeField, Range(2f, 6f)] public int Count { get; set; } = 3;
        protected override Expression Define(ShapeBuilder<ColumnUnionExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Count);
            ExampleInputs.JoinInputs(s, out var a, out var b);
            return s.UnionColumns(a, b, 0.18f, amount);
        }
    }
}
