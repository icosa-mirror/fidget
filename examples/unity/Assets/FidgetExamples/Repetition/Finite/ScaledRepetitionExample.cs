using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    public class ScaledRepetitionExample : FidgetMesh<ScaledRepetitionExample>
    {
        [field: SerializeField, Range(0.12f, 0.32f)] public float Radius { get; set; } = 0.24f;
        [field: SerializeField, Range(0.5f, 1.3f)] public float HeightScale { get; set; } = 0.9f;
        [field: SerializeField, Range(-0.8f, 0.8f)] public float Angle { get; set; } = 0.3f;
        [field: SerializeField, Range(0.38f, 0.55f)] public float CellRadius { get; set; } = 0.45f;

        protected override Expression Define(ShapeBuilder<ScaledRepetitionExample> shape)
        {
            var sphere = shape.Sphere(shape.Parameter(mesh => mesh.Radius));
            var cell = shape.Scale(sphere, 1, shape.Parameter(mesh => mesh.HeightScale), 0.8f);
            var repeated = shape.RepeatX(cell, shape.Parameter(mesh => mesh.CellRadius));
            var tilted = shape.RotateZ(repeated, shape.Parameter(mesh => mesh.Angle));
            // Clip the infinite field to a finite volume inside the meshing bounds.
            return shape.Intersect(tilted, shape.Box(new Vector3(1.3f, 0.85f, 0.7f)));
        }
    }
}
