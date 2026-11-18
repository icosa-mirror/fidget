using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Cheap versus exact rounding. Each tile is a separate modelling recipe.</summary>
    public class DistanceSolids : FidgetMesh<DistanceSolids>
    {
        [field: SerializeField, Range(0.1f, 0.3f)] public float Rounding { get; set; } = 0.2f;
        protected override Expression Define(ShapeBuilder<DistanceSolids> s)
        {
            var amount = s.Parameter(mesh => mesh.Rounding);
            return GalleryLayout.Tiles(s, s.Round(s.Box(0.4f, 0.4f, 0.4f), amount),
                s.Round(s.BoxExact(new Expression3(0.4f)), amount),
                s.Round(s.Cylinder(0.4f, 0.4f), amount), s.Round(s.CylinderExact(0.4f, 0.4f), amount));
        }
    }
}
