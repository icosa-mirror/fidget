using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed octagon recipe.</summary>
    public class OctagonExample : FidgetMesh<OctagonExample>
    {
        [field: SerializeField, Range(0.3f, 0.6f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<OctagonExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.ExtrudeZExact(s.Octagon(amount), -0.12f, 0.12f);
        }
    }
}
