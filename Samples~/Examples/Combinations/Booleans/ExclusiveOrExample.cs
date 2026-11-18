using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed exclusive or recipe.</summary>
    public class ExclusiveOrExample : FidgetMesh<ExclusiveOrExample>
    {
        [field: SerializeField, Range(0.08f, 1.55f)] public float Separation { get; set; } = 0.5f;
        [field: SerializeField, Range(0.25f, 0.82f)] public float Radius { get; set; } = 0.5f;
        [field: SerializeField, Range(0.035f, 0.3f)] public float HalfDepth { get; set; } = 0.08f;
        protected override Expression Define(ShapeBuilder<ExclusiveOrExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.Separation);
            var disc = s.Circle(radius);
            var left = s.Translate(disc, -amount * 0.5f, 0, 0);
            var right = s.Translate(disc, amount * 0.5f, 0, 0);
            return s.ExtrudeZExact(s.Xor(left, right), -halfDepth, halfDepth);
        }
    }
}
