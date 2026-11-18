using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Stars and adjustable points. Each tile is a separate modelling recipe.</summary>
    public class StarProfiles : FidgetMesh<StarProfiles>
    {
        [field: SerializeField, Range(4f, 9f)] public int Points { get; set; } = 5;
        protected override Expression Define(ShapeBuilder<StarProfiles> s)
        {
            var amount = s.Parameter(mesh => mesh.Points);
            return GalleryLayout.Profiles(s, s.Star(0.65f, amount, 3), s.Star(0.65f, amount, 2),
                s.Pentagram(0.5f), s.Hexagram(0.4f));
        }
    }
}
