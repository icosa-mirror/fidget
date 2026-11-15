using UnityEngine;

namespace Icosa.Fidget.Samples
{
    /// <summary>On-demand updates, driven by Inspector or UI sliders.</summary>
    [RequireComponent(typeof(FidgetMesh))]
    public sealed class InteractiveParameters : MonoBehaviour
    {
        [Range(0.1f, 0.65f)] public float MajorRadius = 0.5f;
        [Range(0.05f, 0.25f)] public float TubeRadius = 0.15f;
        private TorusMesh _mesh;

        public sealed class TorusMesh
        {
            public float MajorRadius { get; set; } = 0.5f;
            public float TubeRadius { get; set; } = 0.15f;

            public static CompiledShape<TorusMesh> Compile()
            {
                var shape = new ShapeBuilder<TorusMesh>();
                return shape.Compile(shape.Torus(shape.Parameter(mesh => mesh.MajorRadius), shape.Parameter(mesh => mesh.TubeRadius)));
            }
        }
        private void Start()
        {
            _mesh = new TorusMesh { MajorRadius = MajorRadius, TubeRadius = TubeRadius };
            var output = GetComponent<FidgetMesh>();
            output.Continuous = false;
            output.Initialize(TorusMesh.Compile(), _mesh, new MeshSettings(new Bounds(Vector3.zero, Vector3.one * 2), output.Detail, MeshNormals.Surface));
        }
        public void SetMajorRadius(float value) => MajorRadius = Mathf.Clamp(value, 0.1f, 0.65f);
        public void SetTubeRadius(float value) => TubeRadius = Mathf.Clamp(value, 0.05f, 0.25f);
        private void Update()
        {
            if (_mesh == null) return;
            _mesh.MajorRadius = MajorRadius;
            _mesh.TubeRadius = TubeRadius;
        }
    }
}
