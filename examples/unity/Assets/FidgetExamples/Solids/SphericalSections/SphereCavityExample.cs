using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed sphere cavity recipe.</summary>
    public class SphereCavityExample : FidgetMesh<SphereCavityExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<SphereCavityExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.SphereCavity(0.6f, 0.45f, 0.45f), amount);
        }
    }
}
