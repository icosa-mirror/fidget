using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Rounded joins. Each tile is a separate modelling recipe.</summary>
    public class RoundedJoins : FidgetMesh<RoundedJoins>
    {
        [field: SerializeField, Range(0.1f, 0.4f)] public float Radius { get; set; } = 0.25f;
        protected override Expression Define(ShapeBuilder<RoundedJoins> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            GalleryLayout.JoinInputs(s, out var a, out var b);
            return GalleryLayout.Tiles(s, s.UnionRound(a, b, amount), s.IntersectRound(a, b, amount), s.SubtractRound(a, b, amount));
        }
    }
}
