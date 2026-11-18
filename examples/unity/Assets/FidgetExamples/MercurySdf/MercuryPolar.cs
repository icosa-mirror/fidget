using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    public class MercuryPolar : FidgetMesh<MercuryPolar>
    {
        [field: SerializeField, Range(3, 12)] public float Repetitions { get; set; } = 7;
        [field: SerializeField, Range(0.07f, 0.2f)] public float Radius { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<MercuryPolar> shape)
        {
            var capsule = shape.Translate(shape.Capsule(shape.Parameter(mesh => mesh.Radius), 0.3f),
                                          new Vector3(0.65f, 0, 0));
            return shape.RepeatPolarY(capsule, shape.Parameter(mesh => mesh.Repetitions));
        }
    }
}
