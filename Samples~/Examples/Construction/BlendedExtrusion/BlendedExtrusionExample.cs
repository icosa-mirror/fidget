using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    public class BlendedExtrusionExample : FidgetMesh<BlendedExtrusionExample>
    {
        [field: SerializeField, Range(0.12f, 0.32f)] public float Radius { get; set; } = 0.24f;
        [field: SerializeField, Range(0.02f, 0.3f)] public float BlendRadius { get; set; } = 0.18f;
        [field: SerializeField, Range(0.25f, 0.65f)] public float Height { get; set; } = 0.45f;

        protected override Expression Define(ShapeBuilder<BlendedExtrusionExample> shape)
        {
            var radius = shape.Parameter(mesh => mesh.Radius);
            var height = shape.Parameter(mesh => mesh.Height);
            var disk = shape.ExtrudeZ(shape.Circle(radius), -height, height);
            var block = shape.ExtrudeZ(shape.Rectangle(new Vector2(-0.22f, -0.22f), new Vector2(0.22f, 0.22f)), -height, height);
            return shape.Blend(shape.Move(disk, -0.22f, 0, 0), shape.Move(block, 0.22f, 0, 0),
                               shape.Parameter(mesh => mesh.BlendRadius));
        }
    }
}
