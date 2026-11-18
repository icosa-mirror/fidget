using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed displaced sphere recipe.</summary>
    public class DisplacedSphereExample : FidgetMesh<DisplacedSphereExample>
    {
        [field: SerializeField, Range(0f, 1.8f)] public float Strength { get; set; } = 0.8f;
        [field: SerializeField, Range(0.28f, 0.78f)] public float Radius { get; set; } = 0.5f;
        [field: SerializeField, Range(2f, 16f)] public float Frequency { get; set; } = 10f;
        protected override Expression Define(ShapeBuilder<DisplacedSphereExample> s)
        {
            var frequency = s.Parameter(mesh => mesh.Frequency);
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.Strength);
            return s.Displace(s.Sphere(radius), (s.X * frequency).Sin() * (s.Y * frequency).Sin() * (s.Z * frequency).Sin() * amount * 0.08f);
        }
    }
}
