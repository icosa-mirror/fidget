using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed heart recipe.</summary>
    public class HeartExample : FidgetMesh<HeartExample>
    {
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<HeartExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            return s.ExtrudeZExact(s.Translate(s.Heart(), 0, -0.25f, 0), -halfDepth, halfDepth);
        }
    }
}
