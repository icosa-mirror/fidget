using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed solid angle recipe.</summary>
    public class SolidAngleExample : FidgetMesh<SolidAngleExample>
    {
        [field: SerializeField, Range(0.4f, 0.95f)] public float HalfAngle { get; set; } = 0.65f;
        protected override Expression Define(ShapeBuilder<SolidAngleExample> s)
        {
            var amount = s.Parameter(mesh => mesh.HalfAngle);
            return s.SolidAngle(0.65f, amount);
        }
    }
}
