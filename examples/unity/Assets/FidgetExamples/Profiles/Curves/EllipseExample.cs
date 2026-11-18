using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed ellipse recipe.</summary>
    public class EllipseExample : FidgetMesh<EllipseExample>
    {
        [field: SerializeField, Range(0.22f, 0.48f)] public float HalfHeight { get; set; } = 0.4f;
        protected override Expression Define(ShapeBuilder<EllipseExample> s)
        {
            var amount = s.Parameter(mesh => mesh.HalfHeight);
            return s.ExtrudeZExact(s.Ellipse(new Expression2(0.6f, amount)), -0.12f, 0.12f);
        }
    }
}
