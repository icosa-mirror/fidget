using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    public class MercuryRound : FidgetMesh<MercuryRound>
    {
        [field: SerializeField, Range(0.05f, 0.55f)] public float Radius { get; set; } = 0.35f;
        protected override Expression Define(ShapeBuilder<MercuryRound> shape)
        {
            var a = shape.BoxExact(new Expression3(0.75f, 0.25f, 0.4f));
            var b = shape.Translate(shape.BoxExact(new Expression3(0.25f, 0.65f, 0.4f)), new Vector3(-0.45f, 0.4f, 0));
            return shape.UnionRound(a, b, shape.Parameter(mesh => mesh.Radius));
        }
    }
}
