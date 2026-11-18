using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed egg recipe.</summary>
    public class EggExample : FidgetMesh<EggExample>
    {
        [field: SerializeField, Range(0.5f, 0.85f)] public float Scale { get; set; } = 0.65f;
        protected override Expression Define(ShapeBuilder<EggExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ExtrudeZExact(s.Translate(s.ScaleUniformExact(s.Egg(0.8f, 0.3f, 0.15f, 0.6f), amount), 0, -0.25f, 0), -0.12f, 0.12f);
        }
    }
}
