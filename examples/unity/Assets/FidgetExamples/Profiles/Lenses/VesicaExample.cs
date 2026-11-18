using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed vesica recipe.</summary>
    public class VesicaExample : FidgetMesh<VesicaExample>
    {
        [field: SerializeField, Range(0.15f, 0.35f)] public float Width { get; set; } = 0.25f;
        protected override Expression Define(ShapeBuilder<VesicaExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Width);
            return s.ExtrudeZExact(s.Vesica(amount, 0.6f), -0.12f, 0.12f);
        }
    }
}
