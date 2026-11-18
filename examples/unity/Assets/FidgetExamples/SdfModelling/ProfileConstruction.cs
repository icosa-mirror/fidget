using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Extrude, revolve, loft, elongate. Each tile is a separate modelling recipe.</summary>
    public class ProfileConstruction : FidgetMesh<ProfileConstruction>
    {
        [field: SerializeField, Range(0.06f, 0.18f)] public float Rounding { get; set; } = 0.1f;
        protected override Expression Define(ShapeBuilder<ProfileConstruction> s)
        {
            var amount = s.Parameter(mesh => mesh.Rounding);
            var profile = s.RoundedRectangle(new Expression2(0.4f, 0.3f), amount);
            return GalleryLayout.Tiles(s, s.ExtrudeZExact(profile, -0.3f, 0.3f),
                s.RevolveY(s.Circle(0.2f), 0.4f), s.LoftZ(s.Circle(0.4f), profile, -0.4f, 0.4f),
                s.Elongate(s.Sphere(0.25f), new Expression3(0, 0.4f, 0)));
        }
    }
}
