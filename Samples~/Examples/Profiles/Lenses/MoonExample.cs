using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed moon recipe.</summary>
    public class MoonExample : FidgetMesh<MoonExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<MoonExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.ExtrudeZExact(s.Moon(0.6f, 0.5f, 0.3f), -0.12f, 0.12f), amount);
        }
    }
}
