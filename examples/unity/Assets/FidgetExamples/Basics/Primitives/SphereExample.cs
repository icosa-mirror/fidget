using Icosa.Fidget;
using UnityEngine;
namespace FidgetExamples
{
    public class SphereExample : FidgetMesh<SphereExample>
    {
        [field: SerializeField, Range(0.25f, 0.7f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<SphereExample> s)
        {
            var radius = s.Parameter(mesh => mesh.Radius);
            return s.Sphere(radius);
        }
    }
}
