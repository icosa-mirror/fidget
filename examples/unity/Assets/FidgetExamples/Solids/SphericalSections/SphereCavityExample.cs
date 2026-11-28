using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed sphere cavity recipe.</summary>
    public class SphereCavityExample : FidgetMesh<SphereCavityExample>
    {
        [field: SerializeField, Range(0.25f, 0.92f)] public float CutterOffset { get; set; } = 0.55f;
        [field: SerializeField, Range(0.45f, 0.83f)] public float Radius { get; set; } = 0.6f;
        [field: SerializeField, Range(0.22f, 0.62f)] public float CutterRadius { get; set; } = 0.45f;
        protected override Expression Define(ShapeBuilder<SphereCavityExample> s)
        {
            var cutterRadius = s.Parameter(mesh => mesh.CutterRadius);
            var radius = s.Parameter(mesh => mesh.Radius);
            var amount = s.Parameter(mesh => mesh.CutterOffset);
            return s.SphereCavity(radius, cutterRadius, amount);
        }
    }
}
