using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Polygonal prisms. Each tile is a separate modelling recipe.</summary>
    public class PrismSolids : FidgetMesh<PrismSolids>
    {
        [field: SerializeField, Range(0.3f, 0.65f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<PrismSolids> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return GalleryLayout.Tiles(s, s.HexagonalPrism(amount, 0.4f),
                s.TriangularPrism(amount, 0.4f), s.RhombusPrism(amount, 0.4f, 0.35f, 0.1f),
                s.Cylinder(new Expression3(-0.4f, -0.4f, 0), new Expression3(0.4f, 0.4f, 0), 0.25f));
        }
    }
}
