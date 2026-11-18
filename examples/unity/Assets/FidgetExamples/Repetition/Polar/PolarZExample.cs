using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Cut a ring of ventilation slots around the local Z axis.</summary>
    public class PolarZExample : RotorExample<PolarZExample>
    {
        [field: SerializeField, Range(1, 16)] public int Count { get; set; } = 8;
        protected override Expression Define(ShapeBuilder<PolarZExample> s)
        {
            var count = s.Parameter(mesh => mesh.Count);
            var disc = Disc(s);
            var slot = Slot(s);
            return s.Subtract(disc, s.RepeatPolarZ(slot, count));
        }
    }
}
