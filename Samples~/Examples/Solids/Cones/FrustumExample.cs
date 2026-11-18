using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed frustum recipe.</summary>
    public class FrustumExample : FidgetMesh<FrustumExample>
    {
        [field: SerializeField, Range(0.25f, 0.65f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<FrustumExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.Frustum(amount, 0.2f, 0.55f);
        }
    }
}
