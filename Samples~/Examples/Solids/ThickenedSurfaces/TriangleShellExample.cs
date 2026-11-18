using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed triangle shell recipe.</summary>
    public class TriangleShellExample : FidgetMesh<TriangleShellExample>
    {
        [field: SerializeField, Range(0.05f, 0.18f)] public float Thickness { get; set; } = 0.1f;
        protected override Expression Define(ShapeBuilder<TriangleShellExample> s)
        {
            var amount = s.Parameter(mesh => mesh.Thickness);
            return s.Shell(s.TriangleSurface(new Expression3(-0.6f, -0.5f, 0), new Expression3(0.6f, -0.5f, 0), new Expression3(0, 0.6f, 0)), amount);
        }
    }
}
