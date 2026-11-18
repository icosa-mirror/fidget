using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Rectangle profiles. Each tile is a separate modelling recipe.</summary>
    public class RectangleProfiles : FidgetMesh<RectangleProfiles>
    {
        [field: SerializeField, Range(0.08f, 0.25f)] public float Rounding { get; set; } = 0.15f;
        protected override Expression Define(ShapeBuilder<RectangleProfiles> s)
        {
            var amount = s.Parameter(mesh => mesh.Rounding);
            return GalleryLayout.Profiles(s, s.RectangleExact(0.6f, 0.45f),
                s.RoundedRectangle(new Expression2(0.6f, 0.45f), new Expression4(amount, 0.05f, 0.2f, 0.1f)),
                s.ChamferedRectangle(new Expression2(0.6f, 0.45f), amount),
                s.OrientedRectangle(new Expression2(-0.4f, -0.3f), new Expression2(0.4f, 0.3f), 0.2f));
        }
    }
}
