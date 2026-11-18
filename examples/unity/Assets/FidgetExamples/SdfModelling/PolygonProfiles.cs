using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Regular polygons. Each tile is a separate modelling recipe.</summary>
    public class PolygonProfiles : FidgetMesh<PolygonProfiles>
    {
        [field: SerializeField, Range(0.3f, 0.6f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<PolygonProfiles> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return GalleryLayout.Profiles(s, s.Pentagon(amount), s.Hexagon(amount), s.Octagon(amount), s.Rhombus(amount, 0.65f));
        }
    }
}
