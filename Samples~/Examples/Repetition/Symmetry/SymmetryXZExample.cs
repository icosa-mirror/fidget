using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed symmetry x z recipe.</summary>
    public class SymmetryXZExample : FidgetMesh<SymmetryXZExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<SymmetryXZExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            var part = s.Translate(s.BoxExact(new Expression3(0.18f, 0.3f, 0.18f)), 0.4f, 0.25f, 0.3f);
            return s.ScaleUniformExact(s.SymmetryXZ(part), amount);
        }
    }
}
