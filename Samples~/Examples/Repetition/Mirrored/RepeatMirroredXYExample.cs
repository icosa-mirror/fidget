using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed repeat mirrored x y recipe.</summary>
    public class RepeatMirroredXYExample : FidgetMesh<RepeatMirroredXYExample>
    {
        [field: SerializeField, Range(0.08f, 0.15f)] public float Radius { get; set; } = 0.11f;
        protected override Expression Define(ShapeBuilder<RepeatMirroredXYExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            var clip = s.BoxExact(new Expression3(0.65f, 0.65f, 0.2f));
            return s.Intersect(s.RepeatMirrorXY(s.Translate(s.RotateZ(s.BoxExact(new Expression3(amount * 0.55f, amount, amount)), 0.45f), 0.04f, 0.03f, 0), new Expression2(0.4f)), clip);
        }
    }
}
