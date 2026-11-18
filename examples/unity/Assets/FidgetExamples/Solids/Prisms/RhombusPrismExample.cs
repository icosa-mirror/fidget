using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rhombus prism recipe.</summary>
    public class RhombusPrismExample : FidgetMesh<RhombusPrismExample>
    {
        [field: SerializeField, Range(0.3f, 0.65f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<RhombusPrismExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.RhombusPrism(amount, 0.4f, 0.35f, 0.1f);
        }
    }
}
