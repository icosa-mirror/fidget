using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed hexagram recipe.</summary>
    public class HexagramExample : FidgetMesh<HexagramExample>
    {
        protected override Expression Define(ShapeBuilder<HexagramExample> s)
        {
            return s.ExtrudeZExact(s.Hexagram(0.4f), -0.12f, 0.12f);
        }
    }
}
