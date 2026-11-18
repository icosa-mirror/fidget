using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed exclusive or recipe.</summary>
    public class ExclusiveOrExample : FidgetMesh<ExclusiveOrExample>
    {
        [field: SerializeField, Range(0.1f, 0.8f)] public float Separation { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<ExclusiveOrExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Separation);
            var disc = s.Circle(0.5f);
            var left = s.Translate(disc, -amount * 0.5f, 0, 0);
            var right = s.Translate(disc, amount * 0.5f, 0, 0);
            return s.ExtrudeZExact(s.Xor(left, right), -0.08f, 0.08f);
        }
    }
}
