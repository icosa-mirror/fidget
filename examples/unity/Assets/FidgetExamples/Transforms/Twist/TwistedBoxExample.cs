using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed twisted box recipe.</summary>
    public class TwistedBoxExample : FidgetMesh<TwistedBoxExample>
    {
        [field: SerializeField, Range(-5.5f, 5.5f)] public float Strength { get; set; } = 0.8f;
        [field: SerializeField, Range(0.16f, 0.6f)] public float HalfWidth { get; set; } = 0.35f;
        [field: SerializeField, Range(0.25f, 0.78f)] public float HalfHeight { get; set; } = 0.55f;
        [field: SerializeField, Range(0.12f, 0.5f)] public float HalfDepth { get; set; } = 0.3f;
        protected override Expression Define(ShapeBuilder<TwistedBoxExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var halfWidth = s.Parameter(mesh => mesh.HalfWidth);
            var amount = s.Parameter(mesh => mesh.Strength);
            var box = s.BoxExact(new Expression3(halfWidth, halfHeight, halfDepth));
            return s.TwistY(box, amount);
        }
    }
}
