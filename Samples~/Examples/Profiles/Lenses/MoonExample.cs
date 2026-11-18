using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed moon recipe.</summary>
    public class MoonExample : FidgetMesh<MoonExample>
    {
        [field: SerializeField, Range(0.38f, 0.72f)] public float Separation { get; set; } = 0.6f;
        protected override Expression Define(ShapeBuilder<MoonExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Separation);
            return s.ExtrudeZExact(s.Moon(0.6f, 0.5f, amount), -0.12f, 0.12f);
        }
    }
}
