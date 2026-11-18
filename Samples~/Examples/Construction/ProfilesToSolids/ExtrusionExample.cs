using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed extrusion recipe.</summary>
    public class ExtrusionExample : FidgetMesh<ExtrusionExample>
    {
        [field: SerializeField, Range(0.06f, 0.18f)] public float Rounding { get; set; } = 0.1f;
        protected override Expression Define(ShapeBuilder<ExtrusionExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Rounding);
            var profile = s.RoundedRectangle(new Expression2(0.4f, 0.3f), amount);
            return s.ExtrudeZExact(profile, -0.3f, 0.3f);
        }
    }
}
