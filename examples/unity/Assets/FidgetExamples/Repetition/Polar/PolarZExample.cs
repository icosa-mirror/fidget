using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed polar z recipe.</summary>
    public class PolarZExample : FidgetMesh<PolarZExample>
    {
        [field: SerializeField, Range(3f, 10f)] public int Count { get; set; } = 7;
        protected override Expression Define(ShapeBuilder<PolarZExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Count);
            return s.RepeatPolarZ(s.Translate(s.Sphere(0.12f), 0.5f, 0, 0), amount);
        }
    }
}
