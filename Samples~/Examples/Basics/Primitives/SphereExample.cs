using Icosa.Fidget;
using UnityEngine;
namespace FidgetExamples
{
    public class SphereExample : FidgetMesh<SphereExample>
    {
        [field: SerializeField, Range(0.12f, 1.05f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<SphereExample> s)
        {
            var radius = s.Parameter(mesh => mesh.Radius);
            return s.Sphere(radius);
        }
    }
}
