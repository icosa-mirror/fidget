using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed loft recipe.</summary>
    public class LoftExample : FidgetMesh<LoftExample>
    {
        [field: SerializeField, Range(0.12f, 0.84f)] public float EndWidth { get; set; } = 0.45f;
        [field: SerializeField, Range(0.12f, 0.67f)] public float StartRadius { get; set; } = 0.4f;
        [field: SerializeField, Range(0.1f, 0.58f)] public float EndHeight { get; set; } = 0.22f;
        [field: SerializeField, Range(0.13f, 0.78f)] public float HalfLength { get; set; } = 0.4f;
        protected override Expression Define(ShapeBuilder<LoftExample> s)
        {
            var halfLength = s.Parameter(mesh => mesh.HalfLength);
            var endHeight = s.Parameter(mesh => mesh.EndHeight);
            var startRadius = s.Parameter(mesh => mesh.StartRadius);
            var amount = s.Parameter(mesh => mesh.EndWidth);
            var end = s.RoundedRectangle(new Expression2(amount, endHeight), 0.06f);
            return s.LoftZ(s.Circle(startRadius), end, -halfLength, halfLength);
        }
    }
}
