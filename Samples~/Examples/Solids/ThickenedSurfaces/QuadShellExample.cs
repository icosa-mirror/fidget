using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed quad shell recipe.</summary>
    public class QuadShellExample : FidgetMesh<QuadShellExample>
    {
        [field: SerializeField, Range(0.05f, 0.18f)] public float Thickness { get; set; } = 0.1f;
        protected override Expression Define(ShapeBuilder<QuadShellExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Thickness);
            return s.Shell(s.QuadSurface(new Expression3(-0.5f, -0.5f, 0), new Expression3(0.5f, -0.5f, 0), new Expression3(0.5f, 0.5f, 0), new Expression3(-0.5f, 0.5f, 0)), amount);
        }
    }
}
