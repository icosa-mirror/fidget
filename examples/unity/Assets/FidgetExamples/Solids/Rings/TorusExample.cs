using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed torus recipe.</summary>
    public class TorusExample : FidgetMesh<TorusExample>
    {
        [field: SerializeField, Range(0.1f, 0.25f)] public float TubeRadius { get; set; } = 0.16f;
        protected override Expression Define(ShapeBuilder<TorusExample> s)
        {
            var amount = s.Parameter(mesh => mesh.TubeRadius);
            return s.Torus(0.5f, amount);
        }
    }
}
