using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed repeat x y z recipe.</summary>
    public class RepeatXYZExample : FidgetMesh<RepeatXYZExample>
    {
        [field: SerializeField, Range(0.08f, 0.15f)] public float Radius { get; set; } = 0.11f;
        [field: SerializeField, Range(0.23f, 0.7f)] public float Spacing { get; set; } = 0.4f;
        protected override Expression Define(ShapeBuilder<RepeatXYZExample> s)
        {
            var spacing = s.Parameter(mesh => mesh.Spacing);
            var amount = s.Parameter(mesh => mesh.Radius);
            var bead = s.Sphere(amount);
            return s.Intersect(s.Repeat(bead, new Expression3(spacing)), s.BoxExact(new Expression3(0.6f)));
        }
    }
}
