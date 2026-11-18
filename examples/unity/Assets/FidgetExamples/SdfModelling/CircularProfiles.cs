using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Pie, cut, arc and ring. Each tile is a separate modelling recipe.</summary>
    public class CircularProfiles : FidgetMesh<CircularProfiles>
    {
        [field: SerializeField, Range(0.5f, 2.4f)] public float Aperture { get; set; } = 1.4f;
        protected override Expression Define(ShapeBuilder<CircularProfiles> s)
        {
            var amount = s.Parameter(mesh => mesh.Aperture);
            return GalleryLayout.Profiles(s, s.Pie(0.6f, amount), s.CutCircle(0.6f, 0.1f),
                s.Arc(0.5f, amount, 0.12f), s.RingSector(0.5f, amount, 0.2f));
        }
    }
}
