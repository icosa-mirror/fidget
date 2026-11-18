using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rectangle recipe.</summary>
    public class RectangleExample : FidgetMesh<RectangleExample>
    {
        [field: SerializeField, Range(0.25f, 0.6f)] public float HalfHeight { get; set; } = 0.45f;
        protected override Expression Define(ShapeBuilder<RectangleExample> s)
        {
            var amount = s.Parameter(mesh => mesh.HalfHeight);
            return s.ExtrudeZExact(s.RectangleExact(0.6f, amount), -0.12f, 0.12f);
        }
    }
}
