using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed clipped infinite cone recipe.</summary>
    public class ClippedInfiniteConeExample : FidgetMesh<ClippedInfiniteConeExample>
    {
        [field: SerializeField, Range(0.3f, 0.65f)] public float HalfAngle { get; set; } = 0.45f;
        [field: SerializeField, Range(0.28f, 0.95f)] public float ClipExtent { get; set; } = 0.6f;
        protected override Expression Define(ShapeBuilder<ClippedInfiniteConeExample> s)
        {
            var clipExtent = s.Parameter(mesh => mesh.ClipExtent);
            var amount = s.Parameter(mesh => mesh.HalfAngle);
            var infinite = s.Intersect(s.InfiniteCone(amount), s.BoxExact(new Expression3(clipExtent)));
            return infinite;
        }
    }
}
