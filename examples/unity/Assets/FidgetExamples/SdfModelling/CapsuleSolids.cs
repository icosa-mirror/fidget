using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Capsules and vesica. Each tile is a separate modelling recipe.</summary>
    public class CapsuleSolids : FidgetMesh<CapsuleSolids>
    {
        [field: SerializeField, Range(0.15f, 0.4f)] public float Radius { get; set; } = 0.25f;
        protected override Expression Define(ShapeBuilder<CapsuleSolids> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return GalleryLayout.Tiles(s, s.Capsule(amount, 0.4f),
                s.Capsule(new Expression3(-0.4f, -0.4f, 0), new Expression3(0.4f, 0.4f, 0), amount),
                s.RoundedCone(new Expression3(0, -0.45f, 0), new Expression3(0, 0.45f, 0), amount, 0.15f),
                s.VesicaSegment(new Expression3(0, -0.6f, 0), new Expression3(0, 0.6f, 0), amount));
        }
    }
}
