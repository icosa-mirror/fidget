using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed vesica segment recipe.</summary>
    public class VesicaSegmentExample : FidgetMesh<VesicaSegmentExample>
    {
        [field: SerializeField, Range(0.15f, 0.4f)] public float Radius { get; set; } = 0.25f;
        protected override Expression Define(ShapeBuilder<VesicaSegmentExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.VesicaSegment(new Expression3(0, -0.6f, 0), new Expression3(0, 0.6f, 0), amount);
        }
    }
}
