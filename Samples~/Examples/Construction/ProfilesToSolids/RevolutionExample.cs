using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed revolution recipe.</summary>
    public class RevolutionExample : FidgetMesh<RevolutionExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<RevolutionExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.RevolveY(s.Circle(0.2f), 0.4f), amount);
        }
    }
}
