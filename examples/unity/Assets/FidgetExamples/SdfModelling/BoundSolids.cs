using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Approximate fields. Each tile is a separate modelling recipe.</summary>
    public class BoundSolids : FidgetMesh<BoundSolids>
    {
        [field: SerializeField, Range(0.35f, 0.65f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<BoundSolids> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return GalleryLayout.Tiles(s, s.EllipsoidBound(new Expression3(amount, 0.65f, 0.35f)),
                s.HexagonalPrismBound(amount, 0.4f), s.TriangularPrismBound(amount, 0.4f),
                s.OctahedronBound(amount));
        }
    }
}
