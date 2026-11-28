using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed solid angle recipe.</summary>
    public class SolidAngleExample : FidgetMesh<SolidAngleExample>
    {
        [field: SerializeField, Range(0.16f, 1.45f)] public float HalfAngle { get; set; } = 0.65f;
        [field: SerializeField, Range(0.22f, 1.05f)] public float Radius { get; set; } = 0.65f;
        protected override Expression Define(ShapeBuilder<SolidAngleExample> s)
        {
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.HalfAngle);
            return s.SolidAngle(radius, amount);
        }
    }
}
