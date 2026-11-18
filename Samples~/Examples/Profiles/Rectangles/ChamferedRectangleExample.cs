using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed chamfered rectangle recipe.</summary>
    public class ChamferedRectangleExample : FidgetMesh<ChamferedRectangleExample>
    {
        [field: SerializeField, Range(0.08f, 0.25f)] public float Rounding { get; set; } = 0.15f;
        protected override Expression Define(ShapeBuilder<ChamferedRectangleExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Rounding);
            return s.ExtrudeZExact(s.ChamferedRectangle(new Expression2(0.6f, 0.45f), amount), -0.12f, 0.12f);
        }
    }
}
