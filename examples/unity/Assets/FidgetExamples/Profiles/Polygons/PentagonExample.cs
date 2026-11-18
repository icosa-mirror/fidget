using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed pentagon recipe.</summary>
    public class PentagonExample : FidgetMesh<PentagonExample>
    {
        [field: SerializeField, Range(0.3f, 0.6f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<PentagonExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.ExtrudeZExact(s.Pentagon(amount), -0.12f, 0.12f);
        }
    }
}
