using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Radial cells and identifiers. Each tile is a separate modelling recipe.</summary>
    public class PolarRepetition : FidgetMesh<PolarRepetition>
    {
        [field: SerializeField, Range(3f, 10f)] public int Count { get; set; } = 7;
        protected override Expression Define(ShapeBuilder<PolarRepetition> s)
        {
            var amount = s.Parameter(mesh => mesh.Count);
            var part = s.Translate(s.Capsule(0.08f, 0.15f), new Vector3(0.5f, 0, 0));
            var radial = s.RepeatPolarY(part, amount, out var cell);
            return GalleryLayout.Tiles(s, s.Round(radial, cell.Modulo(2) * 0.025f),
                s.RepeatPolarZ(s.Translate(s.Sphere(0.12f), 0.5f, 0, 0), amount),
                s.MirrorOctantXY(s.Translate(s.Sphere(0.15f), 0.4f, 0.2f, 0), new Expression2(0)));
        }
    }
}
