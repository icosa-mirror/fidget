using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed tongue recipe.</summary>
    public class TongueExample : FidgetMesh<TongueExample>
    {
        [field: SerializeField, Range(0.08f, 0.22f)] public float Width { get; set; } = 0.15f;
        [field: SerializeField, Range(0.1f, 0.41f)] public float CutterRadius { get; set; } = 0.25f;
        [field: SerializeField, Range(0.035f, 0.3f)] public float Height { get; set; } = 0.15f;
        protected override Expression Define(ShapeBuilder<TongueExample> s)
        {
            var height = s.Parameter(mesh => mesh.Height);
            var cutterRadius = s.Parameter(mesh => mesh.CutterRadius);
            var amount = s.Parameter(mesh => mesh.Width);
            var block = s.BoxExact(new Expression3(0.6f, 0.5f, 0.3f));
            var cutter = s.InfiniteCylinder(cutterRadius);
            return s.Tongue(block, cutter, height, amount);
        }
    }
}
