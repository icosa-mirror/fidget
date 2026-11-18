using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed heart recipe.</summary>
    public class HeartExample : FidgetMesh<HeartExample>
    {
        [field: SerializeField, Range(0.5f, 0.85f)] public float Scale { get; set; } = 0.65f;
        protected override Expression Define(ShapeBuilder<HeartExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ExtrudeZExact(s.Translate(s.ScaleUniformExact(s.Heart(), amount), 0, -0.25f, 0), -0.12f, 0.12f);
        }
    }
}
