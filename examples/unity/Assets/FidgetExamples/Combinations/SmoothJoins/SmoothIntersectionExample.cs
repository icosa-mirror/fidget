using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed smooth intersection recipe.</summary>
    public class SmoothIntersectionExample : FidgetMesh<SmoothIntersectionExample>
    {
        [field: SerializeField, Range(0.1f, 0.5f)] public float BlendRadius { get; set; } = 0.3f;
        [field: SerializeField, Range(0.4f, 0.85f)] public float Radius { get; set; } = 0.5f;
        [field: SerializeField, Range(0.1f, 0.32f)] public float HalfSeparation { get; set; } = 0.25f;
        protected override Expression Define(ShapeBuilder<SmoothIntersectionExample> s)
        {
            var halfSeparation = s.Parameter(mesh => mesh.HalfSeparation);
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.BlendRadius);
            var a = s.Translate(s.Sphere(radius), -halfSeparation, 0, 0);
            var b = s.Translate(s.Sphere(radius), halfSeparation, 0, 0);
            return s.IntersectBlend(a, b, amount);
        }
    }
}
