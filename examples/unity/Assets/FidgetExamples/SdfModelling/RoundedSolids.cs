using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Rounded solids. Each tile is a separate modelling recipe.</summary>
    public class RoundedSolids : FidgetMesh<RoundedSolids>
    {
        [field: SerializeField, Range(0.08f, 0.2f)] public float Radius { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<RoundedSolids> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return GalleryLayout.Tiles(s,
                s.RoundedBox(new Expression3(0.6f, 0.45f, 0.4f), amount),
                s.RoundedCylinder(0.55f, 0.5f, amount),
                s.RoundedX(0.55f, amount),
                s.Shell(s.Sphere(0.55f), amount));
        }
    }
}
