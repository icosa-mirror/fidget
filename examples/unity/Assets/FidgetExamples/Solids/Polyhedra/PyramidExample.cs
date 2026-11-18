using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed pyramid recipe.</summary>
    public class PyramidExample : FidgetMesh<PyramidExample>
    {
        [field: SerializeField, Range(0.12f, 1.05f)] public float Radius { get; set; } = 0.45f;
        [field: SerializeField, Range(0.1f, 1f)] public float HalfHeight { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<PyramidExample> s)
        {
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.Pyramid(amount, halfHeight);
        }
    }
}
