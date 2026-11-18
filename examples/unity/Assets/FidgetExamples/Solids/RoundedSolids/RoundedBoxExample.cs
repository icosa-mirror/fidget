using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rounded box recipe.</summary>
    public class RoundedBoxExample : FidgetMesh<RoundedBoxExample>
    {
        [field: SerializeField, Range(0.08f, 0.2f)] public float Radius { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<RoundedBoxExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.RoundedBox(new Expression3(0.6f, 0.45f, 0.4f), amount);
        }
    }
}
