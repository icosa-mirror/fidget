using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed cool s recipe.</summary>
    public class CoolSExample : FidgetMesh<CoolSExample>
    {
        protected override Expression Define(ShapeBuilder<CoolSExample> s)
        {
            return s.ExtrudeZExact(s.CoolS(), -0.12f, 0.12f);
        }
    }
}
