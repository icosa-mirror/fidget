using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed cut circle recipe.</summary>
    public class CutCircleExample : FidgetMesh<CutCircleExample>
    {
        [field: SerializeField, Range(-0.25f, 0.3f)] public float CutHeight { get; set; } = 0.1f;
        protected override Expression Define(ShapeBuilder<CutCircleExample> s)
        {
            var amount = s.Parameter(mesh => mesh.CutHeight);
            return s.ExtrudeZExact(s.CutCircle(0.6f, amount), -0.12f, 0.12f);
        }
    }
}
