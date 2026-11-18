using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Chamfered joins. Each tile is a separate modelling recipe.</summary>
    public class ChamferedJoins : FidgetMesh<ChamferedJoins>
    {
        [field: SerializeField, Range(0.1f, 0.4f)] public float Radius { get; set; } = 0.25f;
        protected override Expression Define(ShapeBuilder<ChamferedJoins> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            GalleryLayout.JoinInputs(s, out var a, out var b);
            return GalleryLayout.Tiles(s, s.UnionChamfer(a, b, amount), s.IntersectChamfer(a, b, amount), s.SubtractChamfer(a, b, amount));
        }
    }
}
