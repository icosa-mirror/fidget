using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    public class MercuryChamfer : FidgetMesh<MercuryChamfer>
    {
        [field: SerializeField, Range(0.05f, 0.55f)] public float Radius { get; set; } = 0.35f;
        protected override Expression Define(ShapeBuilder<MercuryChamfer> shape)
        {
            var p = HgSdf.Point(shape);
            var a = HgSdf.fBox(p, new Expression3(0.75f, 0.25f, 0.4f));
            var b = HgSdf.fBox(p - new Expression3(-0.45f, 0.4f, 0), new Expression3(0.25f, 0.65f, 0.4f));
            return HgSdf.fOpUnionChamfer(a, b, shape.Parameter(mesh => mesh.Radius));
        }
    }
}
