using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed loft recipe.</summary>
    public class LoftExample : FidgetMesh<LoftExample>
    {
        [field: SerializeField, Range(0.06f, 0.18f)] public float Rounding { get; set; } = 0.1f;
        protected override Expression Define(ShapeBuilder<LoftExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Rounding);
            var profile = s.RoundedRectangle(new Expression2(0.4f, 0.3f), amount);
            return s.LoftZ(s.Circle(0.4f), profile, -0.4f, 0.4f);
        }
    }
}
