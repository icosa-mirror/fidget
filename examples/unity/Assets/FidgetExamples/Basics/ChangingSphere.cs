using Icosa.Fidget;
using UnityEngine;
using UnityEngine.Serialization;

namespace FidgetExamples
{
    public class ChangingSphere : FidgetMesh<ChangingSphere>
    {
        [FormerlySerializedAs("Radius"), Range(0.01f, 0.9f)] public float BaseRadius = 0.6f;
        [Range(0, 0.3f)] public float Amplitude = 0.15f;
        [Min(0)] public float Speed = 1;
        public float Radius { get; set; } = 0.6f;

        protected override Expression Define(ShapeBuilder<ChangingSphere> shape) =>
            shape.Sphere(shape.Parameter(sphere => sphere.Radius));

        private void Update() =>
            Radius = Mathf.Clamp(BaseRadius + Amplitude * Mathf.Sin(Time.time * Speed), 0.01f, 0.99f);
    }
}
