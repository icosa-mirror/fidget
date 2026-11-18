using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed groove recipe.</summary>
    public class GrooveExample : FidgetMesh<GrooveExample>
    {
        [field: SerializeField, Range(0.08f, 0.22f)] public float Width { get; set; } = 0.15f;
        protected override Expression Define(ShapeBuilder<GrooveExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Width);
            var block = s.BoxExact(new Expression3(0.6f, 0.5f, 0.3f));
            var cutter = s.InfiniteCylinder(0.25f);
            return s.Groove(block, cutter, 0.15f, amount);
        }
    }
}
