using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed pyramid recipe.</summary>
    public class PyramidExample : FidgetMesh<PyramidExample>
    {
        [field: SerializeField, Range(0.25f, 0.6f)] public float Radius { get; set; } = 0.45f;
        protected override Expression Define(ShapeBuilder<PyramidExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.Pyramid(amount, 0.5f);
        }
    }
}
