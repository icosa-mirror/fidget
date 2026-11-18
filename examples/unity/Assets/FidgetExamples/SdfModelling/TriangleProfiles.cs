using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Triangles and polygon. Each tile is a separate modelling recipe.</summary>
    public class TriangleProfiles : FidgetMesh<TriangleProfiles>
    {
        [field: SerializeField, Range(0.35f, 0.7f)] public float Width { get; set; } = 0.55f;
        protected override Expression Define(ShapeBuilder<TriangleProfiles> s)
        {
            var amount = s.Parameter(mesh => mesh.Width);
            return GalleryLayout.Profiles(s, s.EquilateralTriangle(amount), s.IsoscelesTriangle(amount, 0.5f),
                s.Triangle(new Expression2(-amount, -0.4f), new Expression2(amount, -0.3f), new Expression2(0.2f, 0.6f)),
                s.Polygon(new Expression2(-amount, -0.4f), new Expression2(amount, -0.4f), new Expression2(0.3f, 0), new Expression2(amount, 0.5f), new Expression2(-amount, 0.5f)));
        }
    }
}
