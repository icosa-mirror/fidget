using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed mirror octant recipe.</summary>
    public class MirrorOctantExample : FidgetMesh<MirrorOctantExample>
    {
        [field: SerializeField, Range(0.24f, 0.48f)] public float SeedOffset { get; set; } = 0.4f;
        protected override Expression Define(ShapeBuilder<MirrorOctantExample> s)
        {
            var amount = s.Parameter(mesh => mesh.SeedOffset);
            return s.MirrorOctantXY(s.Translate(s.Sphere(0.15f), amount, 0.2f, 0), new Expression2(0));
        }
    }
}
