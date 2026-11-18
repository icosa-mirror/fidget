using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed revolution recipe.</summary>
    public class RevolutionExample : FidgetMesh<RevolutionExample>
    {
        [field: SerializeField, Range(0.24f, 0.58f)] public float AxisOffset { get; set; } = 0.4f;
        protected override Expression Define(ShapeBuilder<RevolutionExample> s)
        {
            var amount = s.Parameter(mesh => mesh.AxisOffset);
            return s.RevolveY(s.Circle(0.2f), amount);
        }
    }
}
