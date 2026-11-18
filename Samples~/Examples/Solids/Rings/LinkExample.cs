using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed link recipe.</summary>
    public class LinkExample : FidgetMesh<LinkExample>
    {
        [field: SerializeField, Range(0.1f, 0.25f)] public float TubeRadius { get; set; } = 0.16f;
        [field: SerializeField, Range(0.27f, 0.75f)] public float MajorRadius { get; set; } = 0.35f;
        [field: SerializeField, Range(0f, 0.65f)] public float HalfLength { get; set; } = 0.25f;
        protected override Expression Define(ShapeBuilder<LinkExample> s)
        {
            var halfLength = s.Parameter(mesh => mesh.HalfLength);
            var majorRadius = s.Parameter(mesh => mesh.MajorRadius);
            var amount = s.Parameter(mesh => mesh.TubeRadius);
            return s.Link(majorRadius, amount, halfLength);
        }
    }
}
