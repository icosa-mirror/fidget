using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed link recipe.</summary>
    public class LinkExample : FidgetMesh<LinkExample>
    {
        [field: SerializeField, Range(0.1f, 0.25f)] public float TubeRadius { get; set; } = 0.16f;
        protected override Expression Define(ShapeBuilder<LinkExample> s)
        {
            var amount = s.Parameter(mesh => mesh.TubeRadius);
            return s.Link(0.35f, amount, 0.25f);
        }
    }
}
