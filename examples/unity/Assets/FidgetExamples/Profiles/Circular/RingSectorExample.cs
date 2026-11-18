using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed ring sector recipe.</summary>
    public class RingSectorExample : FidgetMesh<RingSectorExample>
    {
        [field: SerializeField, Range(0.5f, 2.4f)] public float Aperture { get; set; } = 1.4f;
        protected override Expression Define(ShapeBuilder<RingSectorExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Aperture);
            return s.ExtrudeZExact(s.RingSector(0.5f, amount, 0.2f), -0.12f, 0.12f);
        }
    }
}
