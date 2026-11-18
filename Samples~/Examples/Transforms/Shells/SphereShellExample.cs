using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed sphere shell recipe.</summary>
    public class SphereShellExample : FidgetMesh<SphereShellExample>
    {
        [field: SerializeField, Range(0.08f, 0.28f)] public float WallThickness { get; set; } = 0.16f;
        protected override Expression Define(ShapeBuilder<SphereShellExample> s)
        {
            var amount = s.Parameter(mesh => mesh.WallThickness);
            var shell = s.Shell(s.Sphere(0.55f), amount * 0.5f);
            return s.Subtract(shell, s.X.Max(s.Z));
        }
    }
}
