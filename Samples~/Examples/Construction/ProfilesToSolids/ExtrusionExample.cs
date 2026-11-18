using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed extrusion recipe.</summary>
    public class ExtrusionExample : FidgetMesh<ExtrusionExample>
    {
        [field: SerializeField, Range(0.08f, 0.48f)] public float HalfDepth { get; set; } = 0.28f;
        protected override Expression Define(ShapeBuilder<ExtrusionExample> s)
        {
            var amount = s.Parameter(mesh => mesh.HalfDepth);
            return s.ExtrudeZExact(s.RoundedRectangle(new Expression2(0.4f, 0.3f), 0.1f), -amount, amount);
        }
    }
}
