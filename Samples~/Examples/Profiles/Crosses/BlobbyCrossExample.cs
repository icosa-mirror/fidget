using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed blobby cross recipe.</summary>
    public class BlobbyCrossExample : FidgetMesh<BlobbyCrossExample>
    {
        [field: SerializeField, Range(0.25f, 0.5f)] public float Bulge { get; set; } = 0.4f;
        protected override Expression Define(ShapeBuilder<BlobbyCrossExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Bulge);
            return s.ExtrudeZExact(s.ScaleUniformExact(s.BlobbyCross(amount), 0.6f), -0.12f, 0.12f);
        }
    }
}
