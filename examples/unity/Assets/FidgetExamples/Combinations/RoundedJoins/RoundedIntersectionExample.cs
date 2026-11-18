using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rounded intersection recipe.</summary>
    public class RoundedIntersectionExample : FidgetMesh<RoundedIntersectionExample>
    {
        [field: SerializeField, Range(0.1f, 0.4f)] public float Radius { get; set; } = 0.25f;
        protected override Expression Define(ShapeBuilder<RoundedIntersectionExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            ExampleInputs.JoinInputs(s, out var a, out var b);
            return s.IntersectRound(a, b, amount);
        }
    }
}
