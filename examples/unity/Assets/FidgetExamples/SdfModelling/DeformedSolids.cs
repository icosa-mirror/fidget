using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Twist, bend, elongate, displace. Each tile is a separate modelling recipe.</summary>
    public class DeformedSolids : FidgetMesh<DeformedSolids>
    {
        [field: SerializeField, Range(0.3f, 1.4f)] public float Strength { get; set; } = 0.8f;
        protected override Expression Define(ShapeBuilder<DeformedSolids> s)
        {
            var amount = s.Parameter(mesh => mesh.Strength);
            var box = s.BoxExact(new Expression3(0.35f, 0.55f, 0.3f));
            return GalleryLayout.Tiles(s, s.TwistY(box, amount), s.BendZ(box, amount),
                s.ElongateInterior(s.Sphere(0.25f), new Expression3(0.3f, 0.2f, 0)),
                s.Displace(s.Sphere(0.5f), (s.X * 10).Sin() * (s.Y * 10).Sin() * (s.Z * 10).Sin() * amount * 0.08f));
        }
    }
}
