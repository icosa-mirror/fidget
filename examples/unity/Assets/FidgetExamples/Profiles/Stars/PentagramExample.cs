using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed pentagram recipe.</summary>
    public class PentagramExample : FidgetMesh<PentagramExample>
    {
        protected override Expression Define(ShapeBuilder<PentagramExample> s)
        {
            return s.ExtrudeZExact(s.Pentagram(0.5f), -0.12f, 0.12f);
        }
    }
}
