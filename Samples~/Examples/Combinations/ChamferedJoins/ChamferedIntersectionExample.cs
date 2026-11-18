using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed chamfered intersection recipe.</summary>
    public class ChamferedIntersectionExample : FidgetMesh<ChamferedIntersectionExample>
    {
        [field: SerializeField, Range(0.1f, 0.4f)] public float Radius { get; set; } = 0.25f;
        protected override Expression Define(ShapeBuilder<ChamferedIntersectionExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            ExampleInputs.JoinInputs(s, out var a, out var b);
            return s.IntersectChamfer(a, b, amount);
        }
    }
}
