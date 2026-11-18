using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed pipe recipe.</summary>
    public class PipeExample : FidgetMesh<PipeExample>
    {
        [field: SerializeField, Range(0.08f, 0.22f)] public float Width { get; set; } = 0.15f;
        protected override Expression Define(ShapeBuilder<PipeExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Width);
            var block = s.BoxExact(new Expression3(0.6f, 0.5f, 0.3f));
            var cutter = s.InfiniteCylinder(0.25f);
            return s.Pipe(block, cutter, amount);
        }
    }
}
