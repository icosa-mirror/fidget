using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed repeat z recipe.</summary>
    public class RepeatZExample : FidgetMesh<RepeatZExample>
    {
        [field: SerializeField, Range(0.08f, 0.18f)] public float Radius { get; set; } = 0.12f;
        [field: SerializeField, Range(0.12f, 0.48f)] public float HalfSpacing { get; set; } = 0.2f;
        protected override Expression Define(ShapeBuilder<RepeatZExample> s)
        {
            var halfSpacing = s.Parameter(mesh => mesh.HalfSpacing);
            var amount = s.Parameter(mesh => mesh.Radius);
            var bead = s.Sphere(amount);
            var clip = s.BoxExact(new Expression3(0.7f));
            return s.Intersect(s.RepeatZ(bead, halfSpacing), clip);
        }
    }
}
