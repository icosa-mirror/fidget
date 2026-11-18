using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Cones and frusta. Each tile is a separate modelling recipe.</summary>
    public class ConeSolids : FidgetMesh<ConeSolids>
    {
        [field: SerializeField, Range(0.25f, 0.65f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<ConeSolids> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            var infinite = s.Intersect(s.InfiniteCone(0.45f), s.BoxExact(new Expression3(0.6f)));
            return GalleryLayout.Tiles(s, s.Cone(amount, 0.55f),
                s.Frustum(amount, 0.2f, 0.55f), s.RoundedCone(amount, 0.2f, 0.5f), infinite);
        }
    }
}
