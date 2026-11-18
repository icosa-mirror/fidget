using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One independently meshed elongated interior recipe.</summary>
    public class ElongatedInteriorExample : FidgetMesh<ElongatedInteriorExample>
    {
        [field: SerializeField, Range(0f, 0.76f)] public float ElongationX { get; set; } = 0.3f;
        protected override Expression Define(ShapeBuilder<ElongatedInteriorExample> s)
        {
            var amount = s.Parameter(mesh => mesh.ElongationX);
            var field = s.ElongateInterior(s.Sphere(0.25f), new Expression3(amount, 0.2f, 0.2f));
            var section = s.Remap(field, new Expression3(s.X, s.Y, 0));
            var outline = s.Shell(section, 0.016f);
            for (int level = 1; level <= 8; level++)
                outline = outline.Min(s.Shell(section + level * 0.05f, 0.016f));
            return s.ExtrudeZExact(outline, -0.025f, 0.025f);
        }
    }
}
