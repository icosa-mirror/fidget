using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Column joins. Each tile is a separate modelling recipe.</summary>
    public class ColumnJoins : FidgetMesh<ColumnJoins>
    {
        [field: SerializeField, Range(2f, 6f)] public int Count { get; set; } = 3;
        protected override Expression Define(ShapeBuilder<ColumnJoins> s)
        {
            var amount = s.Parameter(mesh => mesh.Count);
            GalleryLayout.JoinInputs(s, out var a, out var b);
            return GalleryLayout.Tiles(s, s.UnionColumns(a, b, 0.4f, amount), s.IntersectColumns(a, b, 0.4f, amount), s.SubtractColumns(a, b, 0.4f, amount));
        }
    }
}
