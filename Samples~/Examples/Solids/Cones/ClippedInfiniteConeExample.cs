using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed clipped infinite cone recipe.</summary>
    public class ClippedInfiniteConeExample : FidgetMesh<ClippedInfiniteConeExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<ClippedInfiniteConeExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            var infinite = s.Intersect(s.InfiniteCone(0.45f), s.BoxExact(new Expression3(0.6f)));
            return s.ScaleUniformExact(infinite, amount);
        }
    }
}
