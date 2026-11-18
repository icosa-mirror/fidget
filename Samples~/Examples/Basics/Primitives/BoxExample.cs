using Icosa.Fidget;
using UnityEngine;
namespace FidgetExamples
{
    public class BoxExample : FidgetMesh<BoxExample>
    {
        [field: SerializeField, Range(0.25f, 0.7f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<BoxExample> s)
        {
            var radius = s.Parameter(mesh => mesh.Radius);
            return s.BoxExact(new Expression3(radius));
        }
    }
}
