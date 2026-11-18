using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed symmetry x z recipe.</summary>
    public class SymmetryXZExample : FidgetMesh<SymmetryXZExample>
    {
        [field: SerializeField, Range(0.24f, 0.48f)] public float SeedOffset { get; set; } = 0.4f;
        protected override Expression Define(ShapeBuilder<SymmetryXZExample> s)
        {
            var amount = s.Parameter(mesh => mesh.SeedOffset);
            var part = s.Translate(s.BoxExact(new Expression3(0.18f, 0.3f, 0.18f)), amount, 0.25f, 0.3f);
            return s.SymmetryXZ(part);
        }
    }
}
