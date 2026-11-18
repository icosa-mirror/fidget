using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rounded cylinder offset recipe.</summary>
    public class RoundedCylinderOffsetExample : FidgetMesh<RoundedCylinderOffsetExample>
    {
        [field: SerializeField, Range(0.1f, 0.3f)] public float Rounding { get; set; } = 0.2f;
        protected override Expression Define(ShapeBuilder<RoundedCylinderOffsetExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Rounding);
            return s.Round(s.CylinderExact(0.4f, 0.4f), amount);
        }
    }
}
