using UnityEngine;

namespace Icosa.Fidget.Samples
{
    /// <summary>One compiled expression, independent stream and parameter snapshot per object.</summary>
    public sealed class SharedShapeInstances : MonoBehaviour
    {
        public Material Material;
        [Range(1, 16)] public int Count = 4;
        [Range(1, 6)] public int Detail = 4;
        private CompiledShape _shape;
        private Parameter _radius;
        private MeshStream[] _streams;
        private ParameterValues[] _values;
        private GameObject[] _objects;
        private void OnEnable()
        {
            try
            {
                var builder = new ShapeBuilder();
                _radius = builder.Parameter("radius", 0.55f);
                _shape = builder.Compile(builder.Subtract(builder.Sphere(_radius), builder.Box(new Vector3(0.22f, 1, 0.22f))));
                int count = Mathf.Clamp(Count, 1, 16);
                _streams = new MeshStream[count]; _values = new ParameterValues[count]; _objects = new GameObject[count];
                for (int i = 0; i < count; ++i)
                {
                    _streams[i] = new MeshStream(_shape, new MeshSettings(new Bounds(Vector3.zero, Vector3.one * 2), Detail, MeshNormals.Smooth));
                    _values[i] = _shape.CreateParameters();
                    var go = _objects[i] = new GameObject($"Shared shape {i + 1}", typeof(MeshFilter), typeof(MeshRenderer));
                    go.transform.SetParent(transform, false);
                    go.transform.localPosition = new Vector3((i - (count - 1) * 0.5f) * 1.6f, 0, 0);
                    go.GetComponent<MeshFilter>().sharedMesh = _streams[i].Mesh;
                    go.GetComponent<MeshRenderer>().sharedMaterial = Material;
                }
            }
            catch { OnDisable(); throw; }
        }
        private void Update()
        {
            for (int i = 0; i < _streams.Length; ++i)
            {
                _streams[i].TryApply();
                _values[i].Set(_radius, 0.55f + Mathf.Sin(Time.time + i) * 0.15f);
                _streams[i].TrySchedule(_values[i]);
            }
        }
        private void OnDisable()
        {
            if (_streams != null) foreach (var stream in _streams) stream?.Dispose();
            if (_objects != null) foreach (var go in _objects) if (go != null) Destroy(go);
            _shape?.Dispose(); _shape = null; _streams = null; _objects = null; _values = null;
        }
    }
}
