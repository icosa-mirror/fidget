using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed ellipse recipe.</summary>
    public class EllipseExample : FidgetMesh<EllipseExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<EllipseExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.ExtrudeZExact(s.Ellipse(new Expression2(0.6f, 0.4f)), -0.12f, 0.12f), amount);
        }
    }
}
