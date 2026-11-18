using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed blobby cross recipe.</summary>
    public class BlobbyCrossExample : FidgetMesh<BlobbyCrossExample>
    {
        [field: SerializeField, Range(0.25f, 0.5f)] public float Bulge { get; set; } = 0.4f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<BlobbyCrossExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var amount = s.Parameter(mesh => mesh.Bulge);
            return s.ExtrudeZExact(s.ScaleUniformExact(s.BlobbyCross(amount), 0.6f), -halfDepth, halfDepth);
        }
    }
}
