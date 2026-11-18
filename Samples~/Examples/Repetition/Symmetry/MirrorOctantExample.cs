using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed mirror octant recipe.</summary>
    public class MirrorOctantExample : FidgetMesh<MirrorOctantExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<MirrorOctantExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.MirrorOctantXY(s.Translate(s.Sphere(0.15f), 0.4f, 0.2f, 0), new Expression2(0)), amount);
        }
    }
}
