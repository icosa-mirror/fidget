using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed pentagram recipe.</summary>
    public class PentagramExample : FidgetMesh<PentagramExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<PentagramExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.ExtrudeZExact(s.Pentagram(0.5f), -0.12f, 0.12f), amount);
        }
    }
}
