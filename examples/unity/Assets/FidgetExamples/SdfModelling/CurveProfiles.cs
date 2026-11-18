using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Ellipse, Bezier and conics. Each tile is a separate modelling recipe.</summary>
    public class CurveProfiles : FidgetMesh<CurveProfiles>
    {
        [field: SerializeField, Range(0.05f, 0.15f)] public float Stroke { get; set; } = 0.09f;
        protected override Expression Define(ShapeBuilder<CurveProfiles> s)
        {
            var amount = s.Parameter(mesh => mesh.Stroke);
            return GalleryLayout.Profiles(s, s.Ellipse(new Expression2(0.6f, 0.4f)),
                s.Round(s.Bezier(new Expression2(-0.6f, -0.3f), new Expression2(0, 0.7f), new Expression2(0.6f, -0.3f)), amount),
                s.Intersect(s.ParabolaSegment(0.6f, 0.5f), s.RectangleExact(0.65f, 0.6f)),
                s.Intersect(s.Hyperbola(0.12f, 0.7f), s.RectangleExact(0.6f, 0.6f)));
        }
    }
}
