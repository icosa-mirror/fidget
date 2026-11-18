using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed solid angle recipe.</summary>
    public class SolidAngleExample : FidgetMesh<SolidAngleExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<SolidAngleExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.SolidAngle(0.65f, 0.8f), amount);
        }
    }
}
