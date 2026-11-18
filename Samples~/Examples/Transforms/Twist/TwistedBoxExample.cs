using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed twisted box recipe.</summary>
    public class TwistedBoxExample : FidgetMesh<TwistedBoxExample>
    {
        [field: SerializeField, Range(0.3f, 1.4f)] public float Strength { get; set; } = 0.8f;
        protected override Expression Define(ShapeBuilder<TwistedBoxExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Strength);
            var box = s.BoxExact(new Expression3(0.35f, 0.55f, 0.3f));
            return s.TwistY(box, amount);
        }
    }
}
