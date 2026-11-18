using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed cool s recipe.</summary>
    public class CoolSExample : FidgetMesh<CoolSExample>
    {
        [field: SerializeField, Range(0.5f, 0.85f)] public float Scale { get; set; } = 0.65f;
        protected override Expression Define(ShapeBuilder<CoolSExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Scale);
            return s.ExtrudeZExact(s.ScaleUniformExact(s.CoolS(), amount), -0.12f, 0.12f);
        }
    }
}
