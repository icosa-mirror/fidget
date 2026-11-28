using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed revolution recipe.</summary>
    public class RevolutionExample : FidgetMesh<RevolutionExample>
    {
        [field: SerializeField, Range(0.21f, 0.94f)] public float AxisOffset { get; set; } = 0.4f;
        [field: SerializeField, Range(0.06f, 0.31f)] public float ProfileRadius { get; set; } = 0.2f;
        protected override Expression Define(ShapeBuilder<RevolutionExample> s)
        {
            var profileRadius = s.Parameter(mesh => mesh.ProfileRadius);
            var amount = s.Parameter(mesh => mesh.AxisOffset);
            return s.RevolveY(s.Circle(profileRadius), amount);
        }
    }
}
