using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Limited, mirrored and grid cells. Each tile is a separate modelling recipe.</summary>
    public class LatticeRepetition : FidgetMesh<LatticeRepetition>
    {
        [field: SerializeField, Range(0.08f, 0.15f)] public float Radius { get; set; } = 0.11f;
        protected override Expression Define(ShapeBuilder<LatticeRepetition> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            var bead = s.Sphere(amount);
            var limited = s.RepeatLimited(bead, new Expression3(0.4f), new Expression3(1), out var cell);
            var clip = s.BoxExact(new Expression3(0.65f, 0.65f, 0.2f));
            return GalleryLayout.Tiles(s, s.Round(limited, cell.x.Modulo(2) * 0.02f),
                s.Intersect(s.RepeatMirrorXY(s.BoxExact(new Expression3(amount, amount * 2, amount)), new Expression2(0.4f)), clip),
                s.Intersect(s.RepeatGridXY(bead, new Expression2(0.4f)), clip),
                s.Intersect(s.Repeat(bead, new Expression3(0.4f)), s.BoxExact(new Expression3(0.6f))));
        }
    }
}
