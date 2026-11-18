using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed inflated box recipe.</summary>
    public class InflatedBoxExample : FidgetMesh<InflatedBoxExample>
    {
        [field: SerializeField, Range(0.1f, 0.3f)] public float Rounding { get; set; } = 0.2f;
        protected override Expression Define(ShapeBuilder<InflatedBoxExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Rounding);
            return s.Round(s.Box(0.4f, 0.4f, 0.4f), amount);
        }
    }
}
