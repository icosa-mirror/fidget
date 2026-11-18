using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed cross bound recipe.</summary>
    public class CrossBoundExample : FidgetMesh<CrossBoundExample>
    {
        [field: SerializeField, Range(0.06f, 0.2f)] public float Rounding { get; set; } = 0.12f;
        [field: SerializeField, Range(0.32f, 0.85f)] public float ArmLength { get; set; } = 0.55f;
        [field: SerializeField, Range(0.09f, 0.3f)] public float ArmWidth { get; set; } = 0.2f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<CrossBoundExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var armWidth = s.Parameter(mesh => mesh.ArmWidth);
            var armLength = s.Parameter(mesh => mesh.ArmLength);
            var amount = s.Parameter(mesh => mesh.Rounding);
            return s.ExtrudeZExact(s.Round(s.CrossBound(new Expression2(armLength, armWidth)), amount), -halfDepth, halfDepth);
        }
    }
}
