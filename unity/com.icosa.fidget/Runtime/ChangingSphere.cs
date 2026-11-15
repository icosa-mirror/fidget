using UnityEngine;

namespace Icosa.Fidget
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public sealed class ChangingSphere : MonoBehaviour
    {
        [Range(1, 8)] public int Detail = 4;
        [Range(0.01f, 0.9f)] public float Radius = 0.6f;
        [Range(0, 0.3f)] public float Amplitude = 0.15f;
        [Min(0)] public float Speed = 1;

        private SphereMeshStream _stream;
        private MeshFilter _filter;
        private Mesh _previousMesh;

        private void OnEnable()
        {
            _filter = GetComponent<MeshFilter>();
            _previousMesh = _filter.sharedMesh;
            _stream = new SphereMeshStream();
            _filter.sharedMesh = _stream.Mesh;
        }

        private void Update()
        {
            _stream.TryApply();
            float radius = Mathf.Clamp(Radius + Amplitude * Mathf.Sin(Time.time * Speed), 0.01f, 0.99f);
            _stream.Schedule(radius, Detail);
        }

        private void OnDisable()
        {
            if (_stream == null) return;
            if (_filter.sharedMesh == _stream.Mesh) _filter.sharedMesh = _previousMesh;
            _stream.Dispose();
            _stream = null;
        }
    }
}
