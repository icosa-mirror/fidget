using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed regular star recipe.</summary>
    public class RegularStarExample : FidgetMesh<RegularStarExample>
    {
        [field: SerializeField, Range(4f, 9f)] public int Points { get; set; } = 5;
        protected override Expression Define(ShapeBuilder<RegularStarExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Points);
            return s.ExtrudeZExact(s.Star(0.65f, amount, 3), -0.12f, 0.12f);
        }
    }
}
