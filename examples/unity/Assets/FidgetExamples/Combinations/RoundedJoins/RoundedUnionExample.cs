using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rounded union recipe.</summary>
    public class RoundedUnionExample : FidgetMesh<RoundedUnionExample>
    {
        [field: SerializeField, Range(0.1f, 0.4f)] public float Radius { get; set; } = 0.25f;
        protected override Expression Define(ShapeBuilder<RoundedUnionExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            ExampleInputs.JoinInputs(s, out var a, out var b);
            return s.UnionRound(a, b, amount);
        }
    }
}
