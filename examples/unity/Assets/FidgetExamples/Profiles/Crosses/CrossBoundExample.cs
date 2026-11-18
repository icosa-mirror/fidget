using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed cross bound recipe.</summary>
    public class CrossBoundExample : FidgetMesh<CrossBoundExample>
    {
        [field: SerializeField, Range(0.06f, 0.2f)] public float Rounding { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<CrossBoundExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Rounding);
            return s.ExtrudeZExact(s.Round(s.CrossBound(new Expression2(0.55f, 0.2f)), amount), -0.12f, 0.12f);
        }
    }
}
