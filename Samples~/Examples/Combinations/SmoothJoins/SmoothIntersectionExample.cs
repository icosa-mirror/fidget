using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed smooth intersection recipe.</summary>
    public class SmoothIntersectionExample : FidgetMesh<SmoothIntersectionExample>
    {
        [field: SerializeField, Range(0.1f, 0.5f)] public float BlendRadius { get; set; } = 0.3f;
        protected override Expression Define(ShapeBuilder<SmoothIntersectionExample> s)
        {
            var amount = s.Parameter(mesh => mesh.BlendRadius);
            var a = s.Translate(s.Sphere(0.5f), -0.25f, 0, 0);
            var b = s.Translate(s.Sphere(0.5f), 0.25f, 0, 0);
            return s.IntersectBlend(a, b, amount);
        }
    }
}
