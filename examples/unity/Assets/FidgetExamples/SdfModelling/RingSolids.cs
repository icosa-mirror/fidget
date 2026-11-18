using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Torus, arc and link. Each tile is a separate modelling recipe.</summary>
    public class RingSolids : FidgetMesh<RingSolids>
    {
        [field: SerializeField, Range(0.1f, 0.25f)] public float TubeRadius { get; set; } = 0.16f;
        protected override Expression Define(ShapeBuilder<RingSolids> s)
        {
            var amount = s.Parameter(mesh => mesh.TubeRadius);
            return GalleryLayout.Tiles(s, s.Torus(0.5f, amount),
                s.CappedTorus(0.5f, amount, 2.2f), s.Link(0.35f, amount, 0.25f),
                s.Round(s.CircleCurve(0.5f), amount));
        }
    }
}
