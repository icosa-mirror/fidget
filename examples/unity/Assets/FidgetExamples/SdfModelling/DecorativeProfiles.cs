using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Egg, heart, S and quadratic. Each tile is a separate modelling recipe.</summary>
    public class DecorativeProfiles : FidgetMesh<DecorativeProfiles>
    {
        [field: SerializeField, Range(0.5f, 0.85f)] public float Scale { get; set; } = 0.65f;
        protected override Expression Define(ShapeBuilder<DecorativeProfiles> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return GalleryLayout.Profiles(s, s.Translate(s.ScaleUniformExact(s.Egg(0.8f, 0.3f, 0.15f, 0.6f), amount), 0, -0.25f, 0),
                s.Translate(s.ScaleUniformExact(s.Heart(), amount), 0, -0.25f, 0),
                s.ScaleUniformExact(s.CoolS(), amount), s.ScaleUniformExact(s.QuadraticCircle(), amount));
        }
    }
}
