using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed mirror y recipe.</summary>
    public class MirrorYExample : FidgetMesh<MirrorYExample>
    {
        [field: SerializeField, Range(0.1f, 0.3f)] public float Offset { get; set; } = 0.2f;
        protected override Expression Define(ShapeBuilder<MirrorYExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Offset);
            var part = s.Translate(s.BoxExact(new Expression3(0.18f, 0.3f, 0.18f)), 0.4f, 0.25f, 0.3f);
            return s.MirrorY(part, amount);
        }
    }
}
