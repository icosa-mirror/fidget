using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed horseshoe recipe.</summary>
    public class HorseshoeExample : FidgetMesh<HorseshoeExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<HorseshoeExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.ExtrudeZExact(s.Horseshoe(0.45f, 1.4f, new Expression2(0.3f, 0.1f)), -0.12f, 0.12f), amount);
        }
    }
}
