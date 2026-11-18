using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rectangle recipe.</summary>
    public class RectangleExample : FidgetMesh<RectangleExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<RectangleExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.ExtrudeZExact(s.RectangleExact(0.6f, 0.45f), -0.12f, 0.12f), amount);
        }
    }
}
