using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Smooth union, intersect, subtract. Each tile is a separate modelling recipe.</summary>
    public class SmoothJoins : FidgetMesh<SmoothJoins>
    {
        [field: SerializeField, Range(0.1f, 0.5f)] public float BlendRadius { get; set; } = 0.3f;
        protected override Expression Define(ShapeBuilder<SmoothJoins> s)
        {
            var amount = s.Parameter(mesh => mesh.BlendRadius);
            var a = s.Translate(s.Sphere(0.5f), -0.25f, 0, 0);
            var b = s.Translate(s.Sphere(0.5f), 0.25f, 0, 0);
            return GalleryLayout.Tiles(s, s.Blend(a, b, amount), s.IntersectBlend(a, b, amount),
                s.SubtractBlend(a, b, amount), s.Xor(a, b));
        }
    }
}
