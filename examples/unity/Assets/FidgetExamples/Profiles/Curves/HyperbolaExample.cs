using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed hyperbola recipe.</summary>
    public class HyperbolaExample : FidgetMesh<HyperbolaExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<HyperbolaExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.ExtrudeZExact(s.Intersect(s.Hyperbola(0.12f, 0.7f), s.RectangleExact(0.6f, 0.6f)), -0.12f, 0.12f), amount);
        }
    }
}
