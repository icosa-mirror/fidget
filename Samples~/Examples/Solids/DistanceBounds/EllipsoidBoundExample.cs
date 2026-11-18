using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed ellipsoid bound recipe.</summary>
    public class EllipsoidBoundExample : FidgetMesh<EllipsoidBoundExample>
    {
        [field: SerializeField, Range(0.35f, 0.65f)] public float Radius { get; set; } = 0.5f;
        [field: SerializeField, Range(0.15f, 1.05f)] public float RadiusY { get; set; } = 0.65f;
        [field: SerializeField, Range(0.12f, 0.9f)] public float RadiusZ { get; set; } = 0.35f;
        protected override Expression Define(ShapeBuilder<EllipsoidBoundExample> s)
        {
            var radiusZ = s.Parameter(mesh => mesh.RadiusZ);
            var radiusY = s.Parameter(mesh => mesh.RadiusY);
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.EllipsoidBound(new Expression3(amount, radiusY, radiusZ));
        }
    }
}
