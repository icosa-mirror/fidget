using System.Collections;
using System.Diagnostics;
using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    public sealed class SamplingComparison : MonoBehaviour
    {
        private const float MinSpacing = 0.001f;
        private const float MaxSpacing = 0.04f;
        [Range(MinSpacing, MaxSpacing)] public float FeatureSpacing = 0.016f;
        [Range(0.055f, 0.22f)] public float BoreDiameter = 0.12f;
        [Range(0.045f, 0.18f)] public float BridgeThickness = 0.085f;
        [Tooltip("Shift the sampling bounds by a fraction of a finest cell on each axis.")]
        [Range(0, 1)] public float GridPhase;
#if FIDGET_CURVED_SAMPLING
        [Range(0f, 0.012f)] public float SurfaceError;
#endif
        public MeshFilter FixedMesh, AdaptiveMesh;
        public TextMesh FixedReadout, AdaptiveReadout;

#if FIDGET_SAMPLING
        private CompiledShape _shape;
        private MeshStream _active;
        private MeshStream _fixed, _adaptive;
        private ParameterValues _values;
        private enum View { Both, Fixed, Adaptive }
        private static readonly string[] ViewNames = { "Both", "Fixed", "Adaptive" };
        private View _view;
        private Vector3 _fixedPosition, _adaptivePosition;

        public static CompiledShape CompileFixture()
        {
            var s = new ShapeBuilder();
            Expression diameter = s.Parameter("boreDiameter", 0.12f);
            Expression thickness = s.Parameter("bridgeThickness", 0.085f);
            var radius = diameter * 0.5f;
            var halfBridge = thickness * 0.5f;
            var pad = s.RoundedBox(new Expression3(0.30f, 0.38f, 0.15f), 0.065f);
            var left = s.Translate(pad, -0.48f, 0, 0);
            var right = s.Translate(pad, 0.48f, 0, 0);
            var bridge = s.Box(0.49f, halfBridge, 0.09f);
            var bore = ((s.X + 0.48f).Square() + s.Y.Square()).Sqrt() - radius;
            return s.Compile(left.Min(right).Min(bridge).Max(-bore));
        }

        public static MeshSettings SamplingSettings(float spacing, float phase, bool adaptive)
        {
            var settings = new MeshSettings(new Bounds(Vector3.zero, Vector3.one * 2),
                normals: MeshNormals.Surface, featureSpacing: spacing, cellBudget: 2000000,
                adaptivePlanes: adaptive);
            float edge = settings.LongestCellEdge(settings.SamplingDepth);
            settings.Bounds.center = new Vector3(1f, 0.5f, 0.25f) * (phase * edge);
            return settings;
        }

        private void OnEnable()
        {
            _fixedPosition = FixedMesh.transform.localPosition;
            _adaptivePosition = AdaptiveMesh.transform.localPosition;
            _shape = CompileFixture();
            _values = _shape.CreateParameters();
            StartCoroutine(Compare());
        }

        private Vector4 Controls => new Vector4(FeatureSpacing, BoreDiameter, BridgeThickness, GridPhase);

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(12, 8, 230, 270), GUI.skin.box);
            GUILayout.Label($"Feature spacing  {FeatureSpacing:F4}");
            // Equal slider distances halve or double the cell size.
            float logSpacing = Mathf.Log(FeatureSpacing, 2);
            float selected = GUILayout.HorizontalSlider(logSpacing, Mathf.Log(MinSpacing, 2), Mathf.Log(MaxSpacing, 2));
            if (selected != logSpacing) FeatureSpacing = Mathf.Pow(2, selected);
            BoreDiameter = Slider("Bore diameter", BoreDiameter, 0.055f, 0.22f);
            BridgeThickness = Slider("Bridge thickness", BridgeThickness, 0.045f, 0.18f);
            GridPhase = Slider("Grid phase", GridPhase, 0, 1);
#if FIDGET_CURVED_SAMPLING
            SurfaceError = Slider("Surface error", SurfaceError, 0, 0.012f);
#endif
            var view = (View)GUILayout.Toolbar((int)_view, ViewNames);
            if (view != _view) SetView(view);
            GUILayout.EndArea();
        }

        private void SetView(View view)
        {
            _view = view;
            bool both = view == View.Both;
            var center = (_fixedPosition + _adaptivePosition) * 0.5f;
            FixedMesh.transform.localPosition = both ? _fixedPosition : center;
            AdaptiveMesh.transform.localPosition = both ? _adaptivePosition : center;
            FixedMesh.gameObject.SetActive(view != View.Adaptive);
            AdaptiveMesh.gameObject.SetActive(view != View.Fixed);
            FixedReadout.gameObject.SetActive(both);
            AdaptiveReadout.gameObject.SetActive(both);
        }

        private static float Slider(string label, float value, float min, float max)
        {
            GUILayout.Label($"{label}  {value:F3}");
            return GUILayout.HorizontalSlider(value, min, max);
        }

        private IEnumerator Compare()
        {
            while (true)
            {
                var controls = Controls;
#if FIDGET_CURVED_SAMPLING
                float requestedError = SurfaceError;
#endif
                _values.Set("boreDiameter", controls.y);
                _values.Set("bridgeThickness", controls.z);
                for (int i = 0; i < 2; ++i)
                {
                    bool adaptive = i == 1;
                    var readout = adaptive ? AdaptiveReadout : FixedReadout;
                    string label = adaptive ? "Adaptive planes" : "Fixed depth";
                    var settings = SamplingSettings(controls.x, controls.w, adaptive);
#if FIDGET_CURVED_SAMPLING
                    if (adaptive && requestedError > 0f)
                    {
                        settings.SurfaceError = requestedError;
                        label = "Adaptive curves";
                    }
#endif
                    _active = new MeshStream(_shape, settings);
                    _active.TrySchedule(_values);
                    double applyMilliseconds = 0;
                    while (_active.IsPending)
                    {
                        long start = Stopwatch.GetTimestamp();
                        if (_active.TryApply())
                            applyMilliseconds = (Stopwatch.GetTimestamp() - start) * (1000.0 / Stopwatch.Frequency);
                        else if (_active.IsPending) yield return null;
                    }
                    if (_active.CellBudgetExceeded)
                    {
                        readout.text = $"{label} · depth {settings.SamplingDepth}\n{_active.LastVisitedCells:N0} cells\nCell budget exhausted";
                        _active.Dispose();
                    }
                    else
                    {
                        var filter = adaptive ? AdaptiveMesh : FixedMesh;
                        filter.sharedMesh = _active.Mesh;
                        if (adaptive) { _adaptive?.Dispose(); _adaptive = _active; }
                        else { _fixed?.Dispose(); _fixed = _active; }
                        readout.text = $"{label} · depth {_active.AppliedDepth}\n{_active.LastVisitedCells:N0} cells · {_active.TriangleCount:N0} triangles";
#if FIDGET_WORKER_TIMING
                        readout.text += $"\n{_active.LastWorkerMilliseconds:F1} ms worker · {applyMilliseconds:F2} ms apply";
#else
                        readout.text += $"\n{applyMilliseconds:F2} ms apply";
#endif
#if FIDGET_CURVED_SAMPLING
                        if (settings.SurfaceError > 0) readout.text += $"\n{_active.LastCurvedLeaves:N0} curved cells";
#endif
                    }
                    _active = null;
                }
                while (Controls.Equals(controls))
                {
#if FIDGET_CURVED_SAMPLING
                    if (SurfaceError != requestedError) break;
#endif
                    yield return null;
                }
            }
        }

        private void OnDisable()
        {
            if (_view != View.Both && FixedMesh && AdaptiveMesh && FixedReadout && AdaptiveReadout) SetView(View.Both);
            StopAllCoroutines();
            if (FixedMesh != null) FixedMesh.sharedMesh = null;
            if (AdaptiveMesh != null) AdaptiveMesh.sharedMesh = null;
            _active?.Dispose(); _fixed?.Dispose(); _adaptive?.Dispose(); _shape?.Dispose();
            _active = _fixed = _adaptive = null;
            _shape = null;
        }
#else
        private void Start() { FixedReadout.text = "Sampling comparison requires Fidget 0.8 or later."; AdaptiveReadout.text = ""; }
#endif
    }
}
