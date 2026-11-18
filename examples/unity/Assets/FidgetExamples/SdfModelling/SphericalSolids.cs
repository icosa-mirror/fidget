using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Cuts and spherical sectors. Each tile is a separate modelling recipe.</summary>
    public class SphericalSolids : FidgetMesh<SphericalSolids>
    {
        [field: SerializeField, Range(-0.15f, 0.35f)] public float CutHeight { get; set; } = 0.1f;
        protected override Expression Define(ShapeBuilder<SphericalSolids> s)
        {
            var amount = s.Parameter(mesh => mesh.CutHeight);
            return GalleryLayout.Tiles(s, s.CutSphere(0.6f, amount),
                s.CutHollowSphere(0.6f, amount, 0.09f), s.SphereCavity(0.6f, 0.45f, 0.45f),
                s.SolidAngle(0.65f, 0.8f));
        }
    }
}
