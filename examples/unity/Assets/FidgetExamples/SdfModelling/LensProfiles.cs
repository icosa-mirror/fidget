using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Lenses, moon and horseshoe. Each tile is a separate modelling recipe.</summary>
    public class LensProfiles : FidgetMesh<LensProfiles>
    {
        [field: SerializeField, Range(0.15f, 0.35f)] public float Width { get; set; } = 0.25f;
        protected override Expression Define(ShapeBuilder<LensProfiles> s)
        {
            var amount = s.Parameter(mesh => mesh.Width);
            return GalleryLayout.Profiles(s, s.Vesica(amount, 0.6f),
                s.Vesica(new Expression2(-0.45f, -0.2f), new Expression2(0.45f, 0.2f), amount),
                s.Moon(0.6f, 0.5f, 0.3f), s.Horseshoe(0.45f, 1.4f, new Expression2(0.3f, 0.1f)));
        }
    }
}
