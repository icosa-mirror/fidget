using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Cut a ring of ventilation slots around the local Y axis.</summary>
    public class PolarYExample : FidgetMesh<PolarYExample>
    {
        [field: SerializeField, Range(3f, 10f)] public int Count { get; set; } = 8;
        protected override Expression Define(ShapeBuilder<PolarYExample> s)
        {
            var count = s.Parameter(mesh => mesh.Count);
            var disc = SingleSlotExample.Disc(s);
            var slot = SingleSlotExample.Slot(s);
            // Lay both the disc and its cutter in XZ so their common axis is Y.
            disc = s.Remap(disc, s.X, s.Z, s.Y);
            slot = s.Remap(slot, s.X, s.Z, s.Y);
            return s.Subtract(disc, s.RepeatPolarY(slot, count));
        }
    }
}
