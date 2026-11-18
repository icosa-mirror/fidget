using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed sphere shell recipe.</summary>
    public class SphereShellExample : FidgetMesh<SphereShellExample>
    {
        [field: SerializeField, Range(0.08f, 0.2f)] public float Radius { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<SphereShellExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.Shell(s.Sphere(0.55f), amount);
        }
    }
}
