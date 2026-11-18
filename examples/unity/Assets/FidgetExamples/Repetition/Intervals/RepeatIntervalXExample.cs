using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed repeat interval x recipe.</summary>
    public class RepeatIntervalXExample : FidgetMesh<RepeatIntervalXExample>
    {
        [field: SerializeField, Range(0.08f, 0.18f)] public float Radius { get; set; } = 0.12f;
        [field: SerializeField, Range(0.2f, 0.85f)] public float Spacing { get; set; } = 0.4f;
        [field: SerializeField, Range(0f, 4f)] public int EachSide { get; set; } = 1;
        protected override Expression Define(ShapeBuilder<RepeatIntervalXExample> s)
        {
            var eachSide = s.Parameter(mesh => mesh.EachSide);
            var spacing = s.Parameter(mesh => mesh.Spacing);
            var amount = s.Parameter(mesh => mesh.Radius);
            var bead = s.Sphere(amount);
            return s.RepeatIntervalX(bead, spacing, -eachSide, eachSide);
        }
    }
}
