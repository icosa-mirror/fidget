using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Polyhedra. Each tile is a separate modelling recipe.</summary>
    public class PolyhedronSolids : FidgetMesh<PolyhedronSolids>
    {
        [field: SerializeField, Range(0.25f, 0.6f)] public float Radius { get; set; } = 0.45f;
        protected override Expression Define(ShapeBuilder<PolyhedronSolids> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return GalleryLayout.Tiles(s, s.Octahedron(amount), s.DodecahedronBound(amount),
                s.IcosahedronBound(amount), s.TruncatedOctahedronBound(amount),
                s.TruncatedIcosahedronBound(amount), s.Pyramid(amount, 0.5f));
        }
    }
}
