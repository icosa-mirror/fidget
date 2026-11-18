using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed cone recipe.</summary>
    public class ConeExample : FidgetMesh<ConeExample>
    {
        [field: SerializeField, Range(0.12f, 1.05f)] public float Radius { get; set; } = 0.5f;
        [field: SerializeField, Range(0.1f, 1.0f)] public float HalfHeight { get; set; } = 0.55f;
        protected override Expression Define(ShapeBuilder<ConeExample> s)
        {
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.Cone(amount, halfHeight);
        }
    }
}
