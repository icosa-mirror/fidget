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
                s.Intersect(s.RepeatMirrorXY(s.Translate(s.RotateZ(s.BoxExact(new Expression3(amount * 0.55f, amount, amount)), 0.45f), 0.04f, 0.03f, 0), new Expression2(0.4f)), clip),
                s.Intersect(s.RepeatGridXY(s.Translate(s.BoxExact(new Expression3(amount * 0.55f, amount * 0.6f, amount)), -0.25f, -0.1f, 0), new Expression2(0.4f)), clip),
                s.Intersect(s.Repeat(bead, new Expression3(0.4f)), s.BoxExact(new Expression3(0.6f))));
        }
    }
}
