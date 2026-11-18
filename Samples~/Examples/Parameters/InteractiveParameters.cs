using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>On-demand updates from Inspector properties or UI sliders.</summary>
    public class InteractiveParameters : FidgetMesh<InteractiveParameters>
    {
        [field: SerializeField, Range(0.1f, 0.65f)] public float MajorRadius { get; set; } = 0.5f;
        [field: SerializeField, Range(0.05f, 0.25f)] public float TubeRadius { get; set; } = 0.15f;

        protected override Expression Define(ShapeBuilder<InteractiveParameters> shape) =>
            shape.Torus(shape.Parameter(mesh => mesh.MajorRadius), shape.Parameter(mesh => mesh.TubeRadius));

        public void SetMajorRadius(float value) => MajorRadius = Mathf.Clamp(value, 0.1f, 0.65f);
        public void SetTubeRadius(float value) => TubeRadius = Mathf.Clamp(value, 0.05f, 0.25f);
    }
}
