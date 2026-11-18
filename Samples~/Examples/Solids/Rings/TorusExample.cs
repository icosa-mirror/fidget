using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed torus recipe.</summary>
    public class TorusExample : FidgetMesh<TorusExample>
    {
        [field: SerializeField, Range(0.1f, 0.25f)] public float TubeRadius { get; set; } = 0.16f;
        [field: SerializeField, Range(0.32f, 1f)] public float MajorRadius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<TorusExample> s)
        {
            var majorRadius = s.Parameter(mesh => mesh.MajorRadius);
            var amount = s.Parameter(mesh => mesh.TubeRadius);
            return s.Torus(majorRadius, amount);
        }
    }
}
