using Icosa.Fidget;
using UnityEngine;
namespace FidgetExamples
{
    public class BoxExample : FidgetMesh<BoxExample>
    {
        [field: UnityEngine.Serialization.FormerlySerializedAs("<Radius>k__BackingField")]
        [field: SerializeField, Range(0.12f, 1.05f)] public float HalfWidth { get; set; } = 0.5f;
        [field: SerializeField, Range(0.08f, 1.05f)] public float HalfHeight { get; set; } = 0.5f;
        [field: SerializeField, Range(0.08f, 1.05f)] public float HalfDepth { get; set; } = 0.5f;
        protected override Expression Define(ShapeBuilder<BoxExample> s)
        {
            var halfDepth = s.Parameter(mesh => mesh.HalfDepth);
            var halfHeight = s.Parameter(mesh => mesh.HalfHeight);
            var halfWidth = s.Parameter(mesh => mesh.HalfWidth);
            return s.BoxExact(new Expression3(halfWidth, halfHeight, halfDepth));
        }
    }
}
