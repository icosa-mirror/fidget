using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed pipe recipe.</summary>
    public class PipeExample : FidgetMesh<PipeExample>
    {
        [field: SerializeField, Range(0.08f, 0.22f)] public float Width { get; set; } = 0.15f;
        [field: SerializeField, Range(0.1f, 0.41f)] public float CutterRadius { get; set; } = 0.25f;
        protected override Expression Define(ShapeBuilder<PipeExample> s)
        {
            var cutterRadius = s.Parameter(mesh => mesh.CutterRadius);
            var amount = s.Parameter(mesh => mesh.Width);
            var block = s.BoxExact(new Expression3(0.6f, 0.5f, 0.3f));
            var cutter = s.InfiniteCylinder(cutterRadius);
            return s.Pipe(block, cutter, amount);
        }
    }
}
