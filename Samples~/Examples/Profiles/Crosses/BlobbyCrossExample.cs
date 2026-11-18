using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed blobby cross recipe.</summary>
    public class BlobbyCrossExample : FidgetMesh<BlobbyCrossExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<BlobbyCrossExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.ExtrudeZExact(s.ScaleUniformExact(s.BlobbyCross(0.4f), 0.6f), -0.12f, 0.12f), amount);
        }
    }
}
