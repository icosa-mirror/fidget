using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed arc recipe.</summary>
    public class ArcExample : FidgetMesh<ArcExample>
    {
        [field: SerializeField, Range(0.5f, 2.4f)] public float Aperture { get; set; } = 1.4f;
        protected override Expression Define(ShapeBuilder<ArcExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Aperture);
            return s.ExtrudeZExact(s.Arc(0.5f, amount, 0.12f), -0.12f, 0.12f);
        }
    }
}
