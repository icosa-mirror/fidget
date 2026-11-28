using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed mirror octant recipe.</summary>
    public class MirrorOctantExample : FidgetMesh<MirrorOctantExample>
    {
        [field: SerializeField, Range(0.24f, 0.48f)] public float SeedOffset { get; set; } = 0.4f;
        [field: SerializeField, Range(0.07f, 0.28f)] public float Radius { get; set; } = 0.15f;
        protected override Expression Define(ShapeBuilder<MirrorOctantExample> s)
        {
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.SeedOffset);
            return s.MirrorOctantXY(s.Translate(s.Sphere(radius), amount, 0.2f, 0), new Expression2(0));
        }
    }
}
