using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed plane fold recipe.</summary>
    public class PlaneFoldExample : FidgetMesh<PlaneFoldExample>
    {
        [field: SerializeField, Range(0.1f, 0.3f)] public float Offset { get; set; } = 0.2f;
        protected override Expression Define(ShapeBuilder<PlaneFoldExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Offset);
            return s.FoldPlane(s.Sphere(0.5f), new Vector3(1, 1, 0), amount);
        }
    }
}
