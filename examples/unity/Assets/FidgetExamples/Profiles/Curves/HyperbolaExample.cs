using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed hyperbola recipe.</summary>
    public class HyperbolaExample : FidgetMesh<HyperbolaExample>
    {
        [field: SerializeField, Range(0.06f, 0.18f)] public float Curvature { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<HyperbolaExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Curvature);
            return s.ExtrudeZExact(s.Intersect(s.Hyperbola(amount, 0.7f), s.RectangleExact(0.6f, 0.6f)), -0.12f, 0.12f);
        }
    }
}
