using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed quadratic circle recipe.</summary>
    public class QuadraticCircleExample : FidgetMesh<QuadraticCircleExample>
    {
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<QuadraticCircleExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            return s.ExtrudeZExact(s.QuadraticCircle(), -halfDepth, halfDepth);
        }
    }
}
