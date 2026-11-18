using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed elongation recipe.</summary>
    public class ElongationExample : FidgetMesh<ElongationExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<ElongationExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.Elongate(s.Sphere(0.25f), new Expression3(0, 0.4f, 0)), amount);
        }
    }
}
