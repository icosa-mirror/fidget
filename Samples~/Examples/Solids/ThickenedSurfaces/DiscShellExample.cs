using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed disc shell recipe.</summary>
    public class DiscShellExample : FidgetMesh<DiscShellExample>
    {
        [field: SerializeField, Range(0.05f, 0.18f)] public float Thickness { get; set; } = 0.1f;
        [field: SerializeField, Range(0.15f, 0.95f)] public float Radius { get; set; } = 0.55f;
        protected override Expression Define(ShapeBuilder<DiscShellExample> s)
        {
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.Thickness);
            return s.Shell(s.DiscSurface(radius), amount);
        }
    }
}
