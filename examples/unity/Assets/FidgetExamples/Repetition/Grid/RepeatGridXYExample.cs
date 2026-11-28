using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed repeat grid x y recipe.</summary>
    public class RepeatGridXYExample : FidgetMesh<RepeatGridXYExample>
    {
        [field: SerializeField, Range(0.08f, 0.15f)] public float Radius { get; set; } = 0.11f;
        [field: SerializeField, Range(0.23f, 0.7f)] public float Spacing { get; set; } = 0.4f;
        protected override Expression Define(ShapeBuilder<RepeatGridXYExample> s)
        {
            var spacing = s.Parameter(mesh => mesh.Spacing);
            var amount = s.Parameter(mesh => mesh.Radius);
            var clip = s.BoxExact(new Expression3(0.65f, 0.65f, 0.2f));
            return s.Intersect(s.RepeatGridXY(s.Translate(s.BoxExact(new Expression3(amount * 0.55f, amount * 0.6f, amount)), -0.25f, -0.1f, 0), new Expression2(spacing)), clip);
        }
    }
}
