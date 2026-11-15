using System;
using UnityEngine;

namespace Icosa.Fidget
{
    /// <summary>Owns a compiled shape and streams it into a MeshRenderer. Initialize once from your script.</summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    [DisallowMultipleComponent]
    public sealed class FidgetMesh : MonoBehaviour
    {
        [Tooltip("When disabled, call RequestUpdate after changing parameters.")]
        public bool Continuous = true;
        [Range(1, 8)] public int Detail = 6;
        public ParameterValues Parameters { get; private set; }
        public MeshStream Stream { get; private set; }
        private CompiledShape _shape;
        private MeshSettings _settings;
        private MeshFilter _filter;
        private Mesh _previousMesh;
        private bool _requested;

        /// <summary>Transfers ownership of shape to this component. Use MeshStream directly to share a shape between objects.</summary>
        public void Initialize(CompiledShape shape, MeshSettings settings)
        {
            if (shape == null) throw new ArgumentNullException(nameof(shape));
            if (ReferenceEquals(shape, _shape)) throw new ArgumentException("Pass a newly compiled shape when reinitializing.", nameof(shape));
            settings.Validate();
            StopStream();
            _shape?.Dispose();
            _shape = shape;
            _settings = settings;
            Detail = settings.Depth;
            Parameters = shape.CreateParameters();
            _requested = true;
            if (isActiveAndEnabled) StartStream();
        }
        public void RequestUpdate() => _requested = true;
        private void StartStream()
        {
            if (_shape == null || Stream != null) return;
            _filter = GetComponent<MeshFilter>();
            _previousMesh = _filter.sharedMesh;
            Stream = new MeshStream(_shape, _settings);
            _filter.sharedMesh = Stream.Mesh;
            _requested = true;
        }
        private void StopStream()
        {
            if (Stream == null) return;
            if (_filter != null && _filter.sharedMesh == Stream.Mesh) _filter.sharedMesh = _previousMesh;
            Stream.Dispose();
            Stream = null;
        }
        private void OnEnable() => StartStream();
        private void LateUpdate()
        {
            if (Stream == null) return;
            Stream.TryApply();
            if ((Continuous || _requested) && Stream.TrySchedule(Parameters, Detail)) _requested = false;
        }
        private void OnDisable() => StopStream();
        private void OnDestroy() { StopStream(); _shape?.Dispose(); _shape = null; }
    }
}
