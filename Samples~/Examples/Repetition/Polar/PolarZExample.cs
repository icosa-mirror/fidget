using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed polar z recipe.</summary>
    public class PolarZExample : FidgetMesh<PolarZExample>
    {
        [field: SerializeField, Range(3f, 10f)] public int Count { get; set; } = 7;
        protected override Expression Define(ShapeBuilder<PolarZExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Count);
            var tooth = s.Translate(s.Cone(0.08f, 0.14f), 0.5f, 0, 0);
            return s.RepeatPolarZ(tooth, amount);
        }
    }
}
