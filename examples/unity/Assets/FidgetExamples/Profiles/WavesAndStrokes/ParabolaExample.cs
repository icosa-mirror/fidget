using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed parabola recipe.</summary>
    public class ParabolaExample : FidgetMesh<ParabolaExample>
    {
        [field: SerializeField, Range(1f, 2f)] public float Curvature { get; set; } = 1.5f;
        protected override Expression Define(ShapeBuilder<ParabolaExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Curvature);
            return s.ExtrudeZExact(s.Intersect(s.Parabola(amount), s.RectangleExact(0.6f, 0.6f)), -0.12f, 0.12f);
        }
    }
}
