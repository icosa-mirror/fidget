using UnityEngine;

namespace Icosa.Fidget.Samples
{
    /// <summary>On-demand updates, driven by Inspector or UI sliders.</summary>
    [RequireComponent(typeof(FidgetMesh))]
    public sealed class InteractiveParameters : MonoBehaviour
    {
        [Range(0.1f, 0.65f)] public float MajorRadius = 0.5f;
        [Range(0.05f, 0.25f)] public float TubeRadius = 0.15f;
        private FidgetMesh _mesh;
        private Parameter _major, _tube;
        private float _lastMajor = -1, _lastTube = -1;
        private void Start()
        {
            var shape = new ShapeBuilder();
            _major = shape.Parameter("majorRadius", MajorRadius);
            _tube = shape.Parameter("tubeRadius", TubeRadius);
            _mesh = GetComponent<FidgetMesh>();
            _mesh.Continuous = false;
            _mesh.Initialize(shape.Compile(shape.Torus(_major, _tube)), new MeshSettings(new Bounds(Vector3.zero, Vector3.one * 2), _mesh.Detail, MeshNormals.Surface));
        }
        public void SetMajorRadius(float value) => MajorRadius = Mathf.Clamp(value, 0.1f, 0.65f);
        public void SetTubeRadius(float value) => TubeRadius = Mathf.Clamp(value, 0.05f, 0.25f);
        private void Update()
        {
            if (_mesh == null || (MajorRadius == _lastMajor && TubeRadius == _lastTube)) return;
            _mesh.Parameters.Set(_major, MajorRadius);
            _mesh.Parameters.Set(_tube, TubeRadius);
            _mesh.RequestUpdate();
            _lastMajor = MajorRadius; _lastTube = TubeRadius;
        }
    }
}
