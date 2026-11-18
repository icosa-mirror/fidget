using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed vesica segment recipe.</summary>
    public class VesicaSegmentExample : FidgetMesh<VesicaSegmentExample>
    {
        [field: SerializeField, Range(0.07f, 0.52f)] public float Radius { get; set; } = 0.25f;
        [field: SerializeField, Range(0.48f, 0.95f)] public float HalfSpan { get; set; } = 0.6f;
        protected override Expression Define(ShapeBuilder<VesicaSegmentExample> s)
        {
            var halfSpan = s.Parameter(mesh => mesh.HalfSpan);
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.VesicaSegment(new Expression3(0, -halfSpan, 0), new Expression3(0, halfSpan, 0), amount);
        }
    }
}
