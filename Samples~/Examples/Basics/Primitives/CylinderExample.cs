using Icosa.Fidget;
using UnityEngine;
namespace FidgetExamples
{
    public class CylinderExample : FidgetMesh<CylinderExample>
    {
        [field: SerializeField, Range(0.25f, 0.7f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<CylinderExample> s)
        {
            var radius = s.Parameter(mesh => mesh.Radius);
            return s.CylinderExact(radius, 0.5f);
        }
    }
}
