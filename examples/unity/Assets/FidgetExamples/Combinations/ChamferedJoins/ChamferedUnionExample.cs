using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed chamfered union recipe.</summary>
    public class ChamferedUnionExample : FidgetMesh<ChamferedUnionExample>
    {
        [field: SerializeField, Range(0f, 0.58f)] public float Radius { get; set; } = 0.25f;
        [field: SerializeField, Range(0.17f, 0.37f)] public float ArmWidth { get; set; } = 0.22f;
        protected override Expression Define(ShapeBuilder<ChamferedUnionExample> s)
        {
            var armWidth = s.Parameter(mesh => mesh.ArmWidth);
            var amount = s.Parameter(mesh => mesh.Radius);
            ExampleInputs.JoinInputs(s, armWidth, out var a, out var b);
            return s.UnionChamfer(a, b, amount);
        }
    }
}
