using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed parallelogram recipe.</summary>
    public class ParallelogramExample : FidgetMesh<ParallelogramExample>
    {
        [field: SerializeField, Range(-0.65f, 0.65f)] public float Skew { get; set; } = 0.2f;
        [field: SerializeField, Range(0.1f, 0.8f)] public float HalfWidth { get; set; } = 0.4f;
        [field: SerializeField, Range(0.1f, 0.85f)] public float HalfHeight { get; set; } = 0.45f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<ParallelogramExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var halfWidth = s.Parameter(mesh => mesh.HalfWidth);
            var amount = s.Parameter(mesh => mesh.Skew);
            return s.ExtrudeZExact(s.Parallelogram(halfWidth, halfHeight, amount), -halfDepth, halfDepth);
        }
    }
}
