using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed sphere shell recipe.</summary>
    public class SphereShellExample : FidgetMesh<SphereShellExample>
    {
        [field: SerializeField, Range(0.06f, 0.38f)] public float WallThickness { get; set; } = 0.16f;
        [field: SerializeField, Range(0.23f, 0.92f)] public float Radius { get; set; } = 0.55f;
        protected override Expression Define(ShapeBuilder<SphereShellExample> s)
        {
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.WallThickness);
            var shell = s.Shell(s.Sphere(radius), amount * 0.5f);
            return s.Subtract(shell, s.X.Max(s.Z));
        }
    }
}
