using System;
using UnityEngine;

namespace Icosa.Fidget
{
    /// <summary>Owns a compiled shape and streams it into a MeshRenderer. Initialize once from your script.</summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    [DisallowMultipleComponent]
    public class FidgetMesh : MonoBehaviour
    {
        [Header("Sampling Resolution")]
        [Tooltip("Octree depth when Feature Spacing is zero. Higher values sample more finely.")]
        [Range(1, MeshSettings.MaxDepth)] public int Detail = 6;
        [Tooltip("Sets the finest cell size in local units and selects Detail automatically. Zero uses the Detail slider.")]
        [Min(0)] public float FeatureSpacing;

        [Header("Adaptive Sampling")]
        [Tooltip("With Surface Error at zero, enable this to stop early on eligible flat patches.")]
        public bool AdaptivePlanes;
        [Tooltip("A positive local-space tolerance lets eligible flat and curved patches stop early. Zero uses the Adaptive Planes toggle.")]
        [Min(0)] public float SurfaceError;

        [Header("Build Limit")]
        [Tooltip("Maximum cells visited per build. Zero uses an automatic limit for spacing-based, adaptive, or depth 9–11 builds.")]
        [Min(0)] public int CellBudget;

        public ParameterValues Parameters { get; private set; }
        public MeshStream Stream { get; private set; }
        private CompiledShape _shape;
        private MeshSettings _settings;
        private MeshFilter _filter;
        private Mesh _previousMesh;
        private bool _requested;
        private int _scheduledDepth;
        private int _scheduledBudget;
        private bool _scheduledAdaptivePlanes;
        private float _scheduledSurfaceError;

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
            FeatureSpacing = settings.FeatureSpacing;
            CellBudget = settings.CellBudget;
            AdaptivePlanes = settings.AdaptivePlanes;
            SurfaceError = settings.SurfaceError;
            Parameters = shape.CreateParameters();
            _requested = true;
            if (isActiveAndEnabled) StartStream();
        }
        /// <summary>Transfers compiled-shape ownership and binds a typed model to this component.</summary>
        public void Initialize<T>(CompiledShape<T> shape, T model, MeshSettings settings) where T : class
        {
            if (shape == null) throw new ArgumentNullException(nameof(shape));
            var parameters = shape.CreateParameters(model);
            Initialize(shape.Shape, settings);
            Parameters = parameters;
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
        protected virtual void OnEnable() => StartStream();
        protected virtual void LateUpdate()
        {
            if (Stream == null) return;
            _settings.Depth = Detail;
            _settings.FeatureSpacing = FeatureSpacing;
            _settings.CellBudget = CellBudget;
            _settings.AdaptivePlanes = AdaptivePlanes;
            _settings.SurfaceError = SurfaceError;
            int depth = _settings.SamplingDepth;
            int budget = _settings.BudgetFor(depth);
            if (depth != _scheduledDepth || budget != _scheduledBudget || AdaptivePlanes != _scheduledAdaptivePlanes || SurfaceError != _scheduledSurfaceError) _requested = true;
            bool wasPending = Stream.IsPending;
            Stream.TryApply();
            if (wasPending && !Stream.IsPending && Stream.CellBudgetExceeded)
                Debug.LogWarning($"Fidget stopped after {Stream.LastVisitedCells} cells on {name}. Increase Cell Budget or reduce the requested detail.", this);
            if (Stream.IsPending) return;
            Parameters.CaptureBindings();
            if (!_requested && Stream.HasScheduledValues(Parameters)) return;
            if (Stream.TrySchedule(Parameters, depth, _settings, false))
            {
                _scheduledDepth = depth;
                _scheduledBudget = budget;
                _scheduledAdaptivePlanes = AdaptivePlanes;
                _scheduledSurfaceError = SurfaceError;
                _requested = false;
            }
        }
        protected virtual void OnDisable() => StopStream();
        protected virtual void OnDestroy() { StopStream(); _shape?.Dispose(); _shape = null; }
    }
}
