using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Thickened open surfaces. Each tile is a separate modelling recipe.</summary>
    public class SurfaceSolids : FidgetMesh<SurfaceSolids>
    {
        [field: SerializeField, Range(0.05f, 0.18f)] public float Thickness { get; set; } = 0.1f;
        protected override Expression Define(ShapeBuilder<SurfaceSolids> s)
        {
            var amount = s.Parameter(mesh => mesh.Thickness);
            return GalleryLayout.Tiles(s,
                s.Shell(s.TriangleSurface(new Expression3(-0.6f, -0.5f, 0), new Expression3(0.6f, -0.5f, 0), new Expression3(0, 0.6f, 0)), amount),
                s.Shell(s.QuadSurface(new Expression3(-0.5f, -0.5f, 0), new Expression3(0.5f, -0.5f, 0), new Expression3(0.5f, 0.5f, 0), new Expression3(-0.5f, 0.5f, 0)), amount),
                s.Shell(s.DiscSurface(0.55f), amount),
                s.Round(s.SegmentSurface(new Expression3(-0.5f, -0.4f, 0), new Expression3(0.5f, 0.4f, 0)), amount));
        }
    }
}
