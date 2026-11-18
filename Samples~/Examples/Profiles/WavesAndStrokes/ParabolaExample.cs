using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed parabola recipe.</summary>
    public class ParabolaExample : FidgetMesh<ParabolaExample>
    {
        [field: SerializeField, Range(0.4f, 3.5f)] public float Curvature { get; set; } = 1.5f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<ParabolaExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var amount = s.Parameter(mesh => mesh.Curvature);
            return s.ExtrudeZExact(s.Intersect(s.Parabola(amount), s.RectangleExact(0.6f, 0.6f)), -halfDepth, halfDepth);
        }
    }
}
