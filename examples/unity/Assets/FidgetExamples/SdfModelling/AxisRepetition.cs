using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Axes and finite intervals. Each tile is a separate modelling recipe.</summary>
    public class AxisRepetition : FidgetMesh<AxisRepetition>
    {
        [field: SerializeField, Range(0.08f, 0.18f)] public float Radius { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<AxisRepetition> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            var bead = s.Sphere(amount);
            var clip = s.BoxExact(new Expression3(0.7f));
            return GalleryLayout.Tiles(s, s.Intersect(s.RepeatX(bead, 0.2f), clip),
                s.Intersect(s.RepeatY(bead, 0.2f), clip), s.Intersect(s.RepeatZ(bead, 0.2f), clip),
                s.RepeatIntervalX(bead, 0.4f, -1, 1));
        }
    }
}
