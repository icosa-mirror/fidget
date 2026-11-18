using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed loft recipe.</summary>
    public class LoftExample : FidgetMesh<LoftExample>
    {
        [field: SerializeField, Range(0.18f, 0.58f)] public float EndWidth { get; set; } = 0.45f;
        protected override Expression Define(ShapeBuilder<LoftExample> s)
        {
            var amount = s.Parameter(mesh => mesh.EndWidth);
            var end = s.RoundedRectangle(new Expression2(amount, 0.22f), 0.06f);
            return s.LoftZ(s.Circle(0.4f), end, -0.4f, 0.4f);
        }
    }
}
