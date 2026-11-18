using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed tunnel recipe.</summary>
    public class TunnelExample : FidgetMesh<TunnelExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<TunnelExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.ExtrudeZExact(s.Tunnel(new Expression2(0.5f, 0.45f)), -0.12f, 0.12f), amount);
        }
    }
}
