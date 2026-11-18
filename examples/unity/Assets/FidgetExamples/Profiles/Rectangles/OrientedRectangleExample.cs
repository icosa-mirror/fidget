using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed oriented rectangle recipe.</summary>
    public class OrientedRectangleExample : FidgetMesh<OrientedRectangleExample>
    {
        [field: SerializeField, Range(0.1f, 0.28f)] public float HalfWidth { get; set; } = 0.2f;
        protected override Expression Define(ShapeBuilder<OrientedRectangleExample> s)
        {
            var amount = s.Parameter(mesh => mesh.HalfWidth);
            return s.ExtrudeZExact(s.OrientedRectangle(new Expression2(-0.4f, -0.3f), new Expression2(0.4f, 0.3f), amount), -0.12f, 0.12f);
        }
    }
}
