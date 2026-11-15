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
        private FidgetMesh _mesh;
        private Parameter _offset, _radius;
        private void Start()
        {
            var shape = new ShapeBuilder();
            _offset = shape.Parameter("cutterOffset", 0);
            _radius = shape.Parameter("cutterRadius", CutterRadius);
            var body = shape.Intersect(shape.Sphere(0.85f), shape.Box(Vector3.one * 0.65f));
            var cutter = shape.Translate(shape.Cylinder(_radius, 1.2f), _offset, 0, 0);
            var solid = shape.Subtract(body, cutter);
            _mesh = GetComponent<FidgetMesh>();
            _mesh.Initialize(shape.Compile(solid), new MeshSettings(new Bounds(Vector3.zero, Vector3.one * 2), _mesh.Detail, MeshNormals.Surface));
        }
        private void Update()
        {
            if (_mesh == null) return;
            _mesh.Parameters.Set(_offset, Mathf.Sin(Time.time * Speed) * Travel);
            _mesh.Parameters.Set(_radius, CutterRadius);
        }
    }
}
