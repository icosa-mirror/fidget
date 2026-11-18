using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed pie recipe.</summary>
    public class PieExample : FidgetMesh<PieExample>
    {
        [field: SerializeField, Range(0.5f, 2.4f)] public float Aperture { get; set; } = 1.4f;
        protected override Expression Define(ShapeBuilder<PieExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Aperture);
            return s.ExtrudeZExact(s.Pie(0.6f, amount), -0.12f, 0.12f);
        }
    }
}
