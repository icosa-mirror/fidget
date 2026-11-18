using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed bent box recipe.</summary>
    public class BentBoxExample : FidgetMesh<BentBoxExample>
    {
        [field: SerializeField, Range(0.3f, 1.4f)] public float Strength { get; set; } = 0.8f;
        protected override Expression Define(ShapeBuilder<BentBoxExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Strength);
            var box = s.BoxExact(new Expression3(0.35f, 0.55f, 0.3f));
            return s.BendZ(box, amount);
        }
    }
}
