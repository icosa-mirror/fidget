using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed frustum recipe.</summary>
    public class FrustumExample : FidgetMesh<FrustumExample>
    {
        [field: SerializeField, Range(0.25f, 0.65f)] public float Radius { get; set; } = 0.5f;
        [field: SerializeField, Range(0.1f, 1.0f)] public float HalfHeight { get; set; } = 0.55f;
        [field: SerializeField, Range(0.06f, 0.58f)] public float UpperRadius { get; set; } = 0.2f;
        protected override Expression Define(ShapeBuilder<FrustumExample> s)
        {
            var upperRadius = s.Parameter(mesh => mesh.UpperRadius);
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.Frustum(amount, upperRadius, halfHeight);
        }
    }
}
