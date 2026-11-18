using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed repeat interval x recipe.</summary>
    public class RepeatIntervalXExample : FidgetMesh<RepeatIntervalXExample>
    {
        [field: SerializeField, Range(0.08f, 0.18f)] public float Radius { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<RepeatIntervalXExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            var bead = s.Sphere(amount);
            return s.RepeatIntervalX(bead, 0.4f, -1, 1);
        }
    }
}
