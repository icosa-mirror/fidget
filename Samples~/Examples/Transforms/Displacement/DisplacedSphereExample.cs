using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed displaced sphere recipe.</summary>
    public class DisplacedSphereExample : FidgetMesh<DisplacedSphereExample>
    {
        [field: SerializeField, Range(0.3f, 1.4f)] public float Strength { get; set; } = 0.8f;
        protected override Expression Define(ShapeBuilder<DisplacedSphereExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Strength);
            return s.Displace(s.Sphere(0.5f), (s.X * 10).Sin() * (s.Y * 10).Sin() * (s.Z * 10).Sin() * amount * 0.08f);
        }
    }
}
