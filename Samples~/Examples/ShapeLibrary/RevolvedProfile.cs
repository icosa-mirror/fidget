using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    public class RevolvedProfile : FidgetMesh<RevolvedProfile>
    {
        [field: SerializeField, Range(0.12f, 0.32f)] public float Radius { get; set; } = 0.24f;

        protected override Expression Define(ShapeBuilder<RevolvedProfile> shape)
        {
            var profile = shape.Circle(shape.Parameter(mesh => mesh.Radius) * 0.65f);
            var ring = shape.RevolveY(profile, 0.52f);
            var tab = shape.Move(shape.Box(new Vector3(0.12f, 0.32f, 0.16f)), 0.52f, 0.12f, 0);
            return shape.Union(ring, tab, shape.ReflectX(tab));
        }
    }
}
