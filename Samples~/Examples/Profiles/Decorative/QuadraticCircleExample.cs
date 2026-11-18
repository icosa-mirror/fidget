using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed quadratic circle recipe.</summary>
    public class QuadraticCircleExample : FidgetMesh<QuadraticCircleExample>
    {
        protected override Expression Define(ShapeBuilder<QuadraticCircleExample> s)
        {
            return s.ExtrudeZExact(s.QuadraticCircle(), -0.12f, 0.12f);
        }
    }
}
