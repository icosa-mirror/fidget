using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed parallelogram recipe.</summary>
    public class ParallelogramExample : FidgetMesh<ParallelogramExample>
    {
        [field: SerializeField, Range(0.05f, 0.4f)] public float Skew { get; set; } = 0.2f;
        protected override Expression Define(ShapeBuilder<ParallelogramExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Skew);
            return s.ExtrudeZExact(s.Parallelogram(0.4f, 0.45f, amount), -0.12f, 0.12f);
        }
    }
}
