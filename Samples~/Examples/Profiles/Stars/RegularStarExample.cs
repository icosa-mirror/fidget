using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed regular star recipe.</summary>
    public class RegularStarExample : FidgetMesh<RegularStarExample>
    {
        [field: SerializeField, Range(3f, 16f)] public int Points { get; set; } = 5;
        [field: SerializeField, Range(0.24f, 0.97f)] public float Radius { get; set; } = 0.65f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        [field: SerializeField, Range(0.05f, 0.85f)] public float Indentation { get; set; } = 0.3333333f;
        protected override Expression Define(ShapeBuilder<RegularStarExample> s)
        {
            var indentation = s.Parameter(mesh => mesh.Indentation);
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.Points);
            return s.ExtrudeZExact(s.Star(radius, amount, 2 + indentation * (amount - 2)), -halfDepth, halfDepth);
        }
    }
}
