using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed polar y recipe.</summary>
    public class PolarYExample : FidgetMesh<PolarYExample>
    {
        [field: SerializeField, Range(3f, 10f)] public int Count { get; set; } = 7;
        protected override Expression Define(ShapeBuilder<PolarYExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Count);
            var part = s.Translate(s.Capsule(0.08f, 0.15f), new Vector3(0.5f, 0, 0));
            var radial = s.RepeatPolarY(part, amount, out var cell);
            return s.Round(radial, cell.Modulo(2) * 0.025f);
        }
    }
}
