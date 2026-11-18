using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Power-norm polyhedra. Each tile is a separate modelling recipe.</summary>
    public class PowerPolyhedra : FidgetMesh<PowerPolyhedra>
    {
        [field: SerializeField, Range(3f, 10f)] public float Exponent { get; set; } = 6f;
        protected override Expression Define(ShapeBuilder<PowerPolyhedra> s)
        {
            var amount = s.Parameter(mesh => mesh.Exponent);
            return GalleryLayout.Tiles(s, s.OctahedronPowerBound(0.55f, amount),
                s.DodecahedronBound(0.45f, amount), s.IcosahedronBound(0.45f, amount),
                s.PolyhedronBound(0.45f, amount, 0, 6));
        }
    }
}
