using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Cut a ring of ventilation slots around the local Z axis.</summary>
    public class PolarZExample : FidgetMesh<PolarZExample>
    {
        [field: SerializeField, Range(3f, 10f)] public int Count { get; set; } = 8;
        protected override Expression Define(ShapeBuilder<PolarZExample> s)
        {
            var count = s.Parameter(mesh => mesh.Count);
            var disc = SingleSlotExample.Disc(s);
            var slot = SingleSlotExample.Slot(s);
            return s.Subtract(disc, s.RepeatPolarZ(slot, count));
        }
    }
}
