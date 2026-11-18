using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Pipe, engrave, groove, tongue. Each tile is a separate modelling recipe.</summary>
    public class SurfaceFeatures : FidgetMesh<SurfaceFeatures>
    {
        [field: SerializeField, Range(0.08f, 0.22f)] public float Width { get; set; } = 0.15f;
        protected override Expression Define(ShapeBuilder<SurfaceFeatures> s)
        {
            var amount = s.Parameter(mesh => mesh.Width);
            var block = s.BoxExact(new Expression3(0.6f, 0.5f, 0.3f));
            var cutter = s.InfiniteCylinder(0.25f);
            return GalleryLayout.Tiles(s, s.Pipe(block, cutter, amount), s.Engrave(block, cutter, amount),
                s.Groove(block, cutter, 0.15f, amount), s.Tongue(block, cutter, 0.15f, amount));
        }
    }
}
