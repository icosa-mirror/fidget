using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed regular polygon from star recipe.</summary>
    public class RegularPolygonFromStarExample : FidgetMesh<RegularPolygonFromStarExample>
    {
        [field: SerializeField, Range(4f, 9f)] public int Points { get; set; } = 5;
        protected override Expression Define(ShapeBuilder<RegularPolygonFromStarExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Points);
            return s.ExtrudeZExact(s.Star(0.65f, amount, 2), -0.12f, 0.12f);
        }
    }
}
