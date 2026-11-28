using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed repeat positive x recipe.</summary>
    public class RepeatPositiveXExample : FidgetMesh<RepeatPositiveXExample>
    {
        [field: SerializeField, Range(0.28f, 0.45f)] public float Spacing { get; set; } = 0.4f;
        [field: SerializeField, Range(0.055f, 0.22f)] public float Radius { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<RepeatPositiveXExample> s)
        {
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.Spacing);
            return s.Intersect(s.RepeatPositiveX(s.Sphere(radius), amount), s.BoxExact(new Expression3(0.65f)));
        }
    }
}
