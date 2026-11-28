using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed tunnel recipe.</summary>
    public class TunnelExample : FidgetMesh<TunnelExample>
    {
        [field: SerializeField, Range(0.08f, 0.92f)] public float WallHeight { get; set; } = 0.45f;
        [field: SerializeField, Range(0.14f, 0.85f)] public float HalfWidth { get; set; } = 0.5f;
        [field: SerializeField, Range(0.04f, 0.32f)] public float HalfDepth { get; set; } = 0.12f;
        protected override Expression Define(ShapeBuilder<TunnelExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var halfWidth = s.Parameter(mesh => mesh.HalfWidth);
            var amount = s.Parameter(mesh => mesh.WallHeight);
            return s.ExtrudeZExact(s.Tunnel(new Expression2(halfWidth, amount)), -halfDepth, halfDepth);
        }
    }
}
