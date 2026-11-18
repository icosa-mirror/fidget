using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed tunnel recipe.</summary>
    public class TunnelExample : FidgetMesh<TunnelExample>
    {
        [field: SerializeField, Range(0.2f, 0.65f)] public float WallHeight { get; set; } = 0.45f;
        protected override Expression Define(ShapeBuilder<TunnelExample> s)
        {
            var amount = s.Parameter(mesh => mesh.WallHeight);
            return s.ExtrudeZExact(s.Tunnel(new Expression2(0.5f, amount)), -0.12f, 0.12f);
        }
    }
}
