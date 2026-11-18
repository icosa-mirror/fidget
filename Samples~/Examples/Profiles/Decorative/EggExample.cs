using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed egg recipe.</summary>
    public class EggExample : FidgetMesh<EggExample>
    {
        [field: SerializeField, Range(0.1f, 0.21f)] public float TipRadius { get; set; } = 0.15f;
        protected override Expression Define(ShapeBuilder<EggExample> s)
        {
            var amount = s.Parameter(mesh => mesh.TipRadius);
            return s.ExtrudeZExact(s.Translate(s.Egg(0.8f, 0.3f, amount, 0.6f), 0, -0.25f, 0), -0.12f, 0.12f);
        }
    }
}
