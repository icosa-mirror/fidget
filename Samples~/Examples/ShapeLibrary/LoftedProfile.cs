using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    public class LoftedProfile : FidgetMesh<LoftedProfile>
    {
        [field: SerializeField, Range(0.12f, 0.32f)] public float Radius { get; set; } = 0.24f;
        [field: SerializeField, Range(0.25f, 0.65f)] public float Height { get; set; } = 0.45f;
        [field: SerializeField, Range(-0.8f, 0.8f)] public float Angle { get; set; } = 0.3f;

        protected override Expression Define(ShapeBuilder<LoftedProfile> shape)
        {
            var circle = shape.Circle(shape.Parameter(mesh => mesh.Radius));
            var rectangle = shape.Rectangle(new Vector2(-0.45f, -0.3f), new Vector2(0.45f, 0.3f));
            var height = shape.Parameter(mesh => mesh.Height);
            var loft = shape.LoftZ(circle, rectangle, -height, height);
            return shape.Rotate(loft, new Vector3(1, 1, 0), shape.Parameter(mesh => mesh.Angle));
        }
    }
}
