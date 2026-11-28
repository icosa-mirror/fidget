using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rhombus recipe.</summary>
    public class RhombusExample : FidgetMesh<RhombusExample>
    {
        [field: SerializeField, Range(0.3f, 0.6f)] public float Radius { get; set; } = 0.5f;
        [field: SerializeField, Range(0.28f, 0.94f)] public float HalfHeight { get; set; } = 0.65f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<RhombusExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.ExtrudeZExact(s.Rhombus(amount, halfHeight), -halfDepth, halfDepth);
        }
    }
}
