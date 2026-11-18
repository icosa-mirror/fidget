using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    public abstract class RotorExample<T> : FidgetMesh<T> where T : RotorExample<T>
    {
        [field: SerializeField, Range(0.025f, 0.18f)] public float HalfThickness { get; set; } = 0.055f;
        [field: SerializeField, Range(0.06f, 0.21f)] public float BoreRadius { get; set; } = 0.14f;
        [field: SerializeField, Range(0.29f, 0.43f)] public float SlotStartRadius { get; set; } = 0.32f;
        [field: SerializeField, Range(0.09f, 0.39f)] public float SlotLength { get; set; } = 0.28f;
        [field: SerializeField, Range(0.027f, 0.053f)] public float SlotHalfWidth { get; set; } = 0.05f;

        protected Expression Disc(ShapeBuilder<T> s)
        {
            var depth = s.Parameter(mesh => mesh.HalfThickness);
            var annulus = s.Subtract(s.Circle(0.74f), s.Circle(s.Parameter(mesh => mesh.BoreRadius)));
            return s.ExtrudeZExact(annulus, -depth, depth);
        }

        protected Expression Slot(ShapeBuilder<T> s)
        {
            var start = s.Parameter(mesh => mesh.SlotStartRadius);
            var length = s.Parameter(mesh => mesh.SlotLength);
            var halfWidth = s.Parameter(mesh => mesh.SlotHalfWidth);
            var segment = s.Segment(new Expression2(start, 0), new Expression2(start + length, length * 0.2f));
            return s.Round(segment, halfWidth);
        }
    }
}
