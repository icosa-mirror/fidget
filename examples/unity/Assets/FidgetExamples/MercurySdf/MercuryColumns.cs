using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    public class MercuryColumns : FidgetMesh<MercuryColumns>
    {
        [field: SerializeField, Range(0.05f, 0.55f)] public float Radius { get; set; } = 0.35f;
        [field: SerializeField, Range(2, 8)] public float Count { get; set; } = 4;
        protected override Expression Define(ShapeBuilder<MercuryColumns> shape)
        {
            var a = shape.BoxExact(new Expression3(0.75f, 0.25f, 0.4f));
            var b = shape.Translate(shape.BoxExact(new Expression3(0.25f, 0.65f, 0.4f)), new Vector3(-0.45f, 0.4f, 0));
            return shape.UnionColumns(a, b, shape.Parameter(mesh => mesh.Radius), shape.Parameter(mesh => mesh.Count));
        }
    }
}
