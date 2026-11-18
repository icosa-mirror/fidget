using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Symmetry and plane folding. Each tile is a separate modelling recipe.</summary>
    public class FoldedSolids : FidgetMesh<FoldedSolids>
    {
        [field: SerializeField, Range(0.1f, 0.3f)] public float Offset { get; set; } = 0.2f;
        protected override Expression Define(ShapeBuilder<FoldedSolids> s)
        {
            var amount = s.Parameter(mesh => mesh.Offset);
            var part = s.Translate(s.BoxExact(new Expression3(0.18f, 0.3f, 0.18f)), 0.4f, 0.25f, 0.3f);
            return GalleryLayout.Tiles(s, s.SymmetryXZ(part), s.MirrorY(part, amount),
                s.FoldPlane(s.Sphere(0.5f), new Vector3(1, 1, 0), amount),
                s.Intersect(s.RepeatPositiveX(s.Sphere(0.12f), 0.4f), s.BoxExact(new Expression3(0.65f))));
        }
    }
}
