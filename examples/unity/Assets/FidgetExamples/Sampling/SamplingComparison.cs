using System.Collections;
using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    public sealed class SamplingComparison : MonoBehaviour
    {
        [Range(0.008f, 0.04f)] public float FeatureSpacing = 0.016f;
        [Range(0.055f, 0.22f)] public float BoreDiameter = 0.12f;
        [Range(0.045f, 0.18f)] public float BridgeThickness = 0.085f;
        [Tooltip("Shift the sampling bounds by a fraction of a finest cell on each axis.")]
        [Range(0, 1)] public float GridPhase;
        public MeshFilter FixedMesh, AdaptiveMesh;
        public TextMesh FixedReadout, AdaptiveReadout;

#if FIDGET_SAMPLING
        private CompiledShape _shape;
        private MeshStream _active;
        private MeshStream _fixed, _adaptive;
        private ParameterValues _values;

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
            _shape = CompileFixture();
            _values = _shape.CreateParameters();
            StartCoroutine(Compare());
        }

        private Vector4 Controls => new Vector4(FeatureSpacing, BoreDiameter, BridgeThickness, GridPhase);

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(12, 8, 230, 205), GUI.skin.box);
            FeatureSpacing = Slider("Feature spacing", FeatureSpacing, 0.008f, 0.04f);
            BoreDiameter = Slider("Bore diameter", BoreDiameter, 0.055f, 0.22f);
            BridgeThickness = Slider("Bridge thickness", BridgeThickness, 0.045f, 0.18f);
            GridPhase = Slider("Grid phase", GridPhase, 0, 1);
            GUILayout.EndArea();
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
                _values.Set("boreDiameter", controls.y);
                _values.Set("bridgeThickness", controls.z);
                for (int i = 0; i < 2; ++i)
                {
                    bool adaptive = i == 1;
                    var readout = adaptive ? AdaptiveReadout : FixedReadout;
                    string label = adaptive ? "Adaptive planes" : "Fixed depth";
                    _active = new MeshStream(_shape, SamplingSettings(controls.x, controls.w, adaptive));
                    _active.TrySchedule(_values);
                    while (_active.IsPending && !_active.TryApply()) yield return null;
                    if (_active.CellBudgetExceeded)
                    {
                        readout.text = $"{label}\nCell budget exhausted";
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
                        readout.text += $"\n{_active.LastWorkerMilliseconds:F1} ms worker time";
#endif
                    }
                    _active = null;
                }
                while (Controls.Equals(controls)) yield return null;
            }
        }

        private void OnDisable()
        {
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
