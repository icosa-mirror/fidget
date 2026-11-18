using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed cool s recipe.</summary>
    public class CoolSExample : FidgetMesh<CoolSExample>
    {
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<CoolSExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            return s.ExtrudeZExact(s.CoolS(), -halfDepth, halfDepth);
        }
    }
}
