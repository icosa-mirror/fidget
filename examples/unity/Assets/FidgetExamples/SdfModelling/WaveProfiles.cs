using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Wave, parabola and strokes. Each tile is a separate modelling recipe.</summary>
    public class WaveProfiles : FidgetMesh<WaveProfiles>
    {
        [field: SerializeField, Range(0.05f, 0.15f)] public float Stroke { get; set; } = 0.09f;
        protected override Expression Define(ShapeBuilder<WaveProfiles> s)
        {
            var amount = s.Parameter(mesh => mesh.Stroke);
            return GalleryLayout.Profiles(s,
                s.Intersect(s.Round(s.CircleWave(0.4f, 0.3f), amount), s.RectangleExact(0.65f, 0.6f)),
                s.Intersect(s.Parabola(1.5f), s.RectangleExact(0.6f, 0.6f)),
                s.Round(s.Segment(new Expression2(-0.5f, -0.4f), new Expression2(0.5f, 0.4f)), amount),
                s.UnevenCapsule(0.3f, 0.15f, 0.4f));
        }
    }
}
