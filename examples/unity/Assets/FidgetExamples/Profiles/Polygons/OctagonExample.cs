using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed octagon recipe.</summary>
    public class OctagonExample : FidgetMesh<OctagonExample>
    {
        [field: SerializeField, Range(0.12f, 1.05f)] public float Radius { get; set; } = 0.5f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<OctagonExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.ExtrudeZExact(s.Octagon(amount), -halfDepth, halfDepth);
        }
    }
}
