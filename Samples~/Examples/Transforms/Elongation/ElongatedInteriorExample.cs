using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed elongated interior recipe.</summary>
    public class ElongatedInteriorExample : FidgetMesh<ElongatedInteriorExample>
    {
        [field: SerializeField, Range(0.7f, 1.3f)] public float Scale { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<ElongatedInteriorExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ScaleUniformExact(s.ElongateInterior(s.Sphere(0.25f), new Expression3(0.3f, 0.2f, 0)), amount);
        }
    }
}
