using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed repeat x y z recipe.</summary>
    public class RepeatXYZExample : FidgetMesh<RepeatXYZExample>
    {
        [field: SerializeField, Range(0.08f, 0.15f)] public float Radius { get; set; } = 0.11f;
        protected override Expression Define(ShapeBuilder<RepeatXYZExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            var bead = s.Sphere(amount);
            return s.Intersect(s.Repeat(bead, new Expression3(0.4f)), s.BoxExact(new Expression3(0.6f)));
        }
    }
}
