using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Trapezoid, skew, tunnel, stairs. Each tile is a separate modelling recipe.</summary>
    public class ArchitecturalProfiles : FidgetMesh<ArchitecturalProfiles>
    {
        [field: SerializeField, Range(0.05f, 0.4f)] public float Skew { get; set; } = 0.2f;
        protected override Expression Define(ShapeBuilder<ArchitecturalProfiles> s)
        {
            var amount = s.Parameter(mesh => mesh.Skew);
            return GalleryLayout.Profiles(s, s.Trapezoid(0.6f, 0.3f, 0.45f), s.Parallelogram(0.4f, 0.45f, amount),
                s.Tunnel(new Expression2(0.5f, 0.45f)), s.Translate(s.Stairs(new Expression2(0.2f, 0.2f), 4), -0.4f, -0.4f, 0));
        }
    }
}
