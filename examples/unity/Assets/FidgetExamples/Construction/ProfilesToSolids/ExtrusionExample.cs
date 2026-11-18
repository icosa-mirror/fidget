using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed extrusion recipe.</summary>
    public class ExtrusionExample : FidgetMesh<ExtrusionExample>
    {
        [field: SerializeField, Range(0.04f, 0.8f)] public float HalfDepth { get; set; } = 0.28f;
        [field: SerializeField, Range(0.18f, 0.75f)] public float HalfWidth { get; set; } = 0.4f;
        [field: SerializeField, Range(0.18f, 0.68f)] public float HalfHeight { get; set; } = 0.3f;
        [field: SerializeField, Range(0.01f, 0.17f)] public float CornerRadius { get; set; } = 0.1f;
        protected override Expression Define(ShapeBuilder<ExtrusionExample> s)
        {
            var cornerRadius = s.Parameter(mesh => mesh.CornerRadius);
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var halfWidth = s.Parameter(mesh => mesh.HalfWidth);
            var amount = s.Parameter(mesh => mesh.HalfDepth);
            return s.ExtrudeZExact(s.RoundedRectangle(new Expression2(halfWidth, halfHeight), cornerRadius), -amount, amount);
        }
    }
}
