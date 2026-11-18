using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed repeat positive x recipe.</summary>
    public class RepeatPositiveXExample : FidgetMesh<RepeatPositiveXExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<RepeatPositiveXExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.Intersect(s.RepeatPositiveX(s.Sphere(0.12f), 0.4f), s.BoxExact(new Expression3(0.65f))), amount);
        }
    }
}
