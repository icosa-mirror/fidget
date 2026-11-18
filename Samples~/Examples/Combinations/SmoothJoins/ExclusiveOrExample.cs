using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed exclusive or recipe.</summary>
    public class ExclusiveOrExample : FidgetMesh<ExclusiveOrExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<ExclusiveOrExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            var a = s.Translate(s.Sphere(0.5f), -0.25f, 0, 0);
            var b = s.Translate(s.Sphere(0.5f), 0.25f, 0, 0);
            return s.ScaleUniformExact(s.Xor(a, b), amount);
        }
    }
}
