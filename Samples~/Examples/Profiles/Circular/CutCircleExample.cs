using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed cut circle recipe.</summary>
    public class CutCircleExample : FidgetMesh<CutCircleExample>
    {
        [field: SerializeField, Range(-0.44f, 0.44f)] public float CutHeight { get; set; } = 0.1f;
        [field: SerializeField, Range(0.48f, 0.96f)] public float Radius { get; set; } = 0.6f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<CutCircleExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.CutHeight);
            return s.ExtrudeZExact(s.CutCircle(radius, amount), -halfDepth, halfDepth);
        }
    }
}
