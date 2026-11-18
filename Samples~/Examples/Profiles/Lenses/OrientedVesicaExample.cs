using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed oriented vesica recipe.</summary>
    public class OrientedVesicaExample : FidgetMesh<OrientedVesicaExample>
    {
        [field: SerializeField, Range(0.15f, 0.35f)] public float Width { get; set; } = 0.25f;
        protected override Expression Define(ShapeBuilder<OrientedVesicaExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Width);
            return s.ExtrudeZExact(s.Vesica(new Expression2(-0.45f, -0.2f), new Expression2(0.45f, 0.2f), amount), -0.12f, 0.12f);
        }
    }
}
