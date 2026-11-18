using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Stair joins. Each tile is a separate modelling recipe.</summary>
    public class StairJoins : FidgetMesh<StairJoins>
    {
        [field: SerializeField, Range(2f, 6f)] public int Count { get; set; } = 3;
        protected override Expression Define(ShapeBuilder<StairJoins> s)
        {
            var amount = s.Parameter(mesh => mesh.Count);
            GalleryLayout.JoinInputs(s, out var a, out var b);
            return GalleryLayout.Tiles(s, s.UnionStairs(a, b, 0.4f, amount), s.IntersectStairs(a, b, 0.4f, amount), s.SubtractStairs(a, b, 0.4f, amount));
        }
    }
}
