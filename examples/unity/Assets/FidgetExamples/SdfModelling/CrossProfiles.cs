using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Cross profiles. Each tile is a separate modelling recipe.</summary>
    public class CrossProfiles : FidgetMesh<CrossProfiles>
    {
        [field: SerializeField, Range(0.06f, 0.2f)] public float Rounding { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<CrossProfiles> s)
        {
            var amount = s.Parameter(mesh => mesh.Rounding);
            return GalleryLayout.Profiles(s, s.ScaleUniformExact(s.RoundedCross(0.6f), 0.6f),
                s.Round(s.CrossBound(new Expression2(0.55f, 0.2f)), amount),
                s.RoundedX(0.6f, amount), s.ScaleUniformExact(s.BlobbyCross(0.4f), 0.6f));
        }
    }
}
