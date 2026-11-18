using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed hexagram recipe.</summary>
    public class HexagramExample : FidgetMesh<HexagramExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<HexagramExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.ExtrudeZExact(s.Hexagram(0.4f), -0.12f, 0.12f), amount);
        }
    }
}
