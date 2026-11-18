using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed sphere cavity recipe.</summary>
    public class SphereCavityExample : FidgetMesh<SphereCavityExample>
    {
        [field: SerializeField, Range(0.25f, 0.92f)] public float CutterOffset { get; set; } = 0.55f;
        protected override Expression Define(ShapeBuilder<SphereCavityExample> s)
        {
            var amount = s.Parameter(mesh => mesh.CutterOffset);
            return s.SphereCavity(0.6f, 0.45f, amount);
        }
    }
}
