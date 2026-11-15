using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Define once; animate the cutter through ordinary C# properties.</summary>
    public class AnimatedBoolean : FidgetMesh<AnimatedBoolean>
    {
        [field: SerializeField, Range(0.1f, 0.8f)] public float CutterRadius { get; set; } = 0.32f;
        public float CutterOffset { get; set; }
        [Range(0, 1)] public float Travel = 0.4f;
        public float Speed = 1;

        protected override Expression Define(ShapeBuilder<AnimatedBoolean> shape)
        {
            var offset = shape.Parameter(mesh => mesh.CutterOffset);
            var radius = shape.Parameter(mesh => mesh.CutterRadius);
            var body = shape.Intersect(shape.Sphere(0.85f), shape.Box(Vector3.one * 0.65f));
            var cutter = shape.Translate(shape.Cylinder(radius, 1.2f), offset, 0, 0);
            return shape.Subtract(body, cutter);
        }

        private void Update() => CutterOffset = Mathf.Sin(Time.time * Speed) * Travel;
    }
}
