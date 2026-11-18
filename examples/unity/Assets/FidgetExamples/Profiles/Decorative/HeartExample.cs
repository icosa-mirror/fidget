using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed heart recipe.</summary>
    public class HeartExample : FidgetMesh<HeartExample>
    {
        protected override Expression Define(ShapeBuilder<HeartExample> s)
        {
            return s.ExtrudeZExact(s.Translate(s.Heart(), 0, -0.25f, 0), -0.12f, 0.12f);
        }
    }
}
