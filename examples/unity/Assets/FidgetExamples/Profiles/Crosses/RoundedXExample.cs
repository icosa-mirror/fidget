using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rounded x recipe.</summary>
    public class RoundedXExample : FidgetMesh<RoundedXExample>
    {
        [field: SerializeField, Range(0.06f, 0.2f)] public float Rounding { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<RoundedXExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Rounding);
            return s.ExtrudeZExact(s.RoundedX(0.6f, amount), -0.12f, 0.12f);
        }
    }
}
