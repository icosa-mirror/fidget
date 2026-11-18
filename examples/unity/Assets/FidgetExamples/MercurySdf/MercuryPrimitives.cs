using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    public class MercuryPrimitives : FidgetMesh<MercuryPrimitives>
    {
        [field: SerializeField, Range(0f, 10f)] public float Bob { get; set; } = 1f;
        [field: SerializeField, Range(0.2f, 0.55f)] public float ConeRadius { get; set; } = 0.4f;
        [field: SerializeField, Range(0.12f, 0.3f)] public float CapsuleRadius { get; set; } = 0.22f;
        protected override Expression Define(ShapeBuilder<MercuryPrimitives> shape)
        {
            var cone = shape.Translate(shape.Cone(shape.Parameter(mesh => mesh.ConeRadius), 0.55f), new Vector3(-0.5f, 0, 0));
            var capsule = shape.Capsule( new Expression3(0.5f, -0.35f, 0), new Expression3(0.5f, 0.35f, 0),
                                         shape.Parameter(mesh => mesh.CapsuleRadius));
            var sphere = shape.Sphere(0.5f);
            var iq = shape.TwistY(shape.DodecahedronBound(1), shape.Parameter(mesh => mesh.Bob));
            return iq;
            //return shape.Union(cone, capsule);
        }
    }
}
