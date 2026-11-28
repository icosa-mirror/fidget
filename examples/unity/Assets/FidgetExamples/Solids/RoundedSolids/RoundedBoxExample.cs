using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rounded box recipe.</summary>
    public class RoundedBoxExample : FidgetMesh<RoundedBoxExample>
    {
        [field: SerializeField, Range(0.08f, 0.2f)] public float Radius { get; set; } = 0.12f;
        [field: SerializeField, Range(0.25f, 0.95f)] public float HalfWidth { get; set; } = 0.6f;
        [field: SerializeField, Range(0.25f, 0.95f)] public float HalfHeight { get; set; } = 0.45f;
        [field: SerializeField, Range(0.25f, 0.9f)] public float HalfDepth { get; set; } = 0.4f;
        protected override Expression Define(ShapeBuilder<RoundedBoxExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var halfWidth = s.Parameter(mesh => mesh.HalfWidth);
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.RoundedBox(new Expression3(halfWidth, halfHeight, halfDepth), amount);
        }
    }
}
