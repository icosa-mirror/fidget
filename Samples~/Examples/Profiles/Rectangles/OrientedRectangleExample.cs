using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed oriented rectangle recipe.</summary>
    public class OrientedRectangleExample : FidgetMesh<OrientedRectangleExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<OrientedRectangleExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.ExtrudeZExact(s.OrientedRectangle(new Expression2(-0.4f, -0.3f), new Expression2(0.4f, 0.3f), 0.2f), -0.12f, 0.12f), amount);
        }
    }
}
