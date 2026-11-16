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
            var p = HgSdf.Point(shape);
            var radial = p.xz;
            HgSdf.pModPolar(ref radial, shape.Parameter(mesh => mesh.Repetitions));
            p.xz = radial;
            p.x -= 0.65f;
            return HgSdf.fCapsule(p, shape.Parameter(mesh => mesh.Radius), 0.3f);
        }
    }
}
