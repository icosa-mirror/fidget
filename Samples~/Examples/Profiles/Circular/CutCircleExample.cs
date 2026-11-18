using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed cut circle recipe.</summary>
    public class CutCircleExample : FidgetMesh<CutCircleExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<CutCircleExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.ExtrudeZExact(s.CutCircle(0.6f, 0.1f), -0.12f, 0.12f), amount);
        }
    }
}
