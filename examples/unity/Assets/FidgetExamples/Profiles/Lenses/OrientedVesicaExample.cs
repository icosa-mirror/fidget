using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed oriented vesica recipe.</summary>
    public class OrientedVesicaExample : FidgetMesh<OrientedVesicaExample>
    {
        [field: SerializeField, Range(0.15f, 0.35f)] public float Width { get; set; } = 0.25f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        [field: SerializeField, Range(0.12f, 0.85f)] public float HalfSpan { get; set; } = 0.45f;
        protected override Expression Define(ShapeBuilder<OrientedVesicaExample> s)
        {
            var halfSpan = s.Parameter(mesh => mesh.HalfSpan);
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var amount = s.Parameter(mesh => mesh.Width);
            return s.ExtrudeZExact(s.Vesica(new Expression2(-halfSpan, -0.2f), new Expression2(halfSpan, 0.2f), amount), -halfDepth, halfDepth);
        }
    }
}
