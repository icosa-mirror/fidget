using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed ellipsoid bound recipe.</summary>
    public class EllipsoidBoundExample : FidgetMesh<EllipsoidBoundExample>
    {
        [field: SerializeField, Range(0.35f, 0.65f)] public float Radius { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<EllipsoidBoundExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Radius);
            return s.EllipsoidBound(new Expression3(amount, 0.65f, 0.35f));
        }
    }
}
