using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed cone recipe.</summary>
    public class ConeExample : FidgetMesh<ConeExample>
    {
        [field: SerializeField, Range(0.25f, 0.65f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<ConeExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.Cone(amount, 0.55f);
        }
    }
}
