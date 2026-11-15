using UnityEngine;

namespace Icosa.Fidget.Samples
{
    /// <summary>Build once; animate a cutter parameter without rebuilding the expression.</summary>
    [RequireComponent(typeof(FidgetMesh))]
    public sealed class AnimatedBoolean : MonoBehaviour
    {
        [Range(0.1f, 0.8f)] public float CutterRadius = 0.32f;
        [Range(0, 1)] public float Travel = 0.4f;
        public float Speed = 1;
        private CutterMesh _mesh;

        public sealed class CutterMesh
        {
            public float CutterOffset { get; set; }
            public float CutterRadius { get; set; } = 0.32f;

            public static CompiledShape<CutterMesh> Compile()
            {
                var shape = new ShapeBuilder<CutterMesh>();
                var offset = shape.Parameter(mesh => mesh.CutterOffset);
                var radius = shape.Parameter(mesh => mesh.CutterRadius);
                var body = shape.Intersect(shape.Sphere(0.85f), shape.Box(Vector3.one * 0.65f));
                var cutter = shape.Translate(shape.Cylinder(radius, 1.2f), offset, 0, 0);
                return shape.Compile(shape.Subtract(body, cutter));
            }
        }
        private void Start()
        {
            _mesh = new CutterMesh { CutterRadius = CutterRadius };
            var output = GetComponent<FidgetMesh>();
            output.Initialize(CutterMesh.Compile(), _mesh, new MeshSettings(new Bounds(Vector3.zero, Vector3.one * 2), output.Detail, MeshNormals.Surface));
        }
        private void Update()
        {
            if (_mesh == null) return;
            _mesh.CutterOffset = Mathf.Sin(Time.time * Speed) * Travel;
            _mesh.CutterRadius = CutterRadius;
        }
    }
}
