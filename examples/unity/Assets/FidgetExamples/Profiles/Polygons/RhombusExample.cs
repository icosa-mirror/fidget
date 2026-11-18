using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed rhombus recipe.</summary>
    public class RhombusExample : FidgetMesh<RhombusExample>
    {
        [field: SerializeField, Range(0.3f, 0.6f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<RhombusExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.ExtrudeZExact(s.Rhombus(amount, 0.65f), -0.12f, 0.12f);
        }
    }
}
