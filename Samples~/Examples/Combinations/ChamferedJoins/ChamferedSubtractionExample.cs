using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed chamfered subtraction recipe.</summary>
    public class ChamferedSubtractionExample : FidgetMesh<ChamferedSubtractionExample>
    {
        [field: SerializeField, Range(0.1f, 0.4f)] public float Radius { get; set; } = 0.25f;
        protected override Expression Define(ShapeBuilder<ChamferedSubtractionExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            ExampleInputs.JoinInputs(s, out var a, out var b);
            return s.SubtractChamfer(a, b, amount);
        }
    }
}
