using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed vesica recipe.</summary>
    public class VesicaExample : FidgetMesh<VesicaExample>
    {
        [field: SerializeField, Range(0.15f, 0.35f)] public float Width { get; set; } = 0.25f;
        [field: SerializeField, Range(0.28f, 0.94f)] public float HalfHeight { get; set; } = 0.6f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<VesicaExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var amount = s.Parameter(mesh => mesh.Width);
            return s.ExtrudeZExact(s.Vesica(amount, halfHeight), -halfDepth, halfDepth);
        }
    }
}
