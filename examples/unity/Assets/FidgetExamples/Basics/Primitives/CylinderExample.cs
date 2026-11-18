using Icosa.Fidget;
using UnityEngine;
namespace FidgetExamples
{
    public class CylinderExample : FidgetMesh<CylinderExample>
    {
        [field: SerializeField, Range(0.12f, 1.05f)] public float Radius { get; set; } = 0.5f;
        [field: SerializeField, Range(0.08f, 1f)] public float HalfHeight { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<CylinderExample> s)
        {
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var radius = s.Parameter(mesh => mesh.Radius);
            return s.CylinderExact(radius, halfHeight);
        }
    }
}
