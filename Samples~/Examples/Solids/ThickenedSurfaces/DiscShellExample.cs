using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed disc shell recipe.</summary>
    public class DiscShellExample : FidgetMesh<DiscShellExample>
    {
        [field: SerializeField, Range(0.05f, 0.18f)] public float Thickness { get; set; } = 0.1f;
        protected override Expression Define(ShapeBuilder<DiscShellExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Thickness);
            return s.Shell(s.DiscSurface(0.55f), amount);
        }
    }
}
