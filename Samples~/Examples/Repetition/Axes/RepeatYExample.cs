using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed repeat y recipe.</summary>
    public class RepeatYExample : FidgetMesh<RepeatYExample>
    {
        [field: SerializeField, Range(0.08f, 0.18f)] public float Radius { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<RepeatYExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            var bead = s.Sphere(amount);
            var clip = s.BoxExact(new Expression3(0.7f));
            return s.Intersect(s.RepeatY(bead, 0.2f), clip);
        }
    }
}
