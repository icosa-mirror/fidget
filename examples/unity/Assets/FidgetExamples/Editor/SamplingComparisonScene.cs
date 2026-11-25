using System;
using System.IO;
using System.Linq;
using System.Threading;
using Icosa.Fidget;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FidgetExamples.Editor
{
    public static class SamplingComparisonScene
    {
        public const string Name = "Sampling/RoundedBlockComparison";
        public static string Create()
        {
            string root = ExampleSceneLayout.RootFolder();
            string path = $"{root}/Scenes/{Name}.unity";
            if (SceneManager.GetSceneByPath(path).isLoaded) throw new InvalidOperationException($"Close {path} before regenerating it");
            ExampleSceneLayout.Folder($"{root}/Scenes/Sampling");
            var previous = SceneManager.GetActiveScene();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            try
            {
                SceneManager.SetActiveScene(scene);
                ExampleSceneLayout.Environment(2);
                var control = new GameObject("Sampling comparison").AddComponent<SamplingComparison>();
                var material = AssetDatabase.LoadAssetAtPath<Material>($"{root}/Rendering/Fidget Orange.mat");
                for (int i = 0; i < 2; ++i)
                {
                    string title = i == 0 ? "Fixed depth" : "Adaptive planes";
                    var go = new GameObject(title, typeof(MeshFilter), typeof(MeshRenderer));
                    go.transform.SetParent(control.transform, false);
                    go.transform.localPosition = new Vector3(i == 0 ? -1.4f : 1.4f, 0.25f, 0);
                    go.transform.localRotation = Quaternion.Euler(-14, -15, 0);
                    go.transform.localScale = Vector3.one * 1.42f;
                    go.GetComponent<MeshRenderer>().sharedMaterial = material;
                    var text = new GameObject(title + " readout").AddComponent<TextMesh>();
                    text.transform.SetParent(control.transform, false);
                    text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    text.GetComponent<MeshRenderer>().sharedMaterial = text.font.material;
                    text.transform.position = go.transform.position + new Vector3(0, -1.05f, 0);
                    text.anchor = TextAnchor.MiddleCenter;
                    text.alignment = TextAlignment.Center;
                    text.fontSize = 42;
                    text.characterSize = 0.034f;
                    text.text = title;
                    if (i == 0) { control.FixedMesh = go.GetComponent<MeshFilter>(); control.FixedReadout = text; }
                    else { control.AdaptiveMesh = go.GetComponent<MeshFilter>(); control.AdaptiveReadout = text; }
                }
                if (!EditorSceneManager.SaveScene(scene, path)) throw new IOException($"Could not save {path}");
            }
            finally
            {
                if (previous.IsValid()) SceneManager.SetActiveScene(previous);
                EditorSceneManager.CloseScene(scene, true);
            }
            return path;
        }

        public static void Generate() => Debug.Log($"[FidgetCoupon] Saved {Create()}");

        public static string Validate()
        {
#if FIDGET_SAMPLING
            var scene = EditorSceneManager.OpenPreviewScene($"{ExampleSceneLayout.RootFolder()}/Scenes/{Name}.unity");
            try
            {
                var controls = scene.GetRootGameObjects().Select(go => go.GetComponent<SamplingComparison>()).Single(c => c != null);
                if (!controls.FixedReadout || !controls.AdaptiveReadout || !controls.FixedMesh || !controls.AdaptiveMesh)
                    throw new InvalidDataException("Missing comparison scene references");
                using (var shape = SamplingComparison.CompileFixture())
                {
                    var values = shape.CreateParameters();
                    foreach (float phase in new[] { 0f, 0.37f, 0.81f })
                    foreach (var (adaptive, error) in Modes())
                    {
                        var settings = SamplingComparison.SamplingSettings(error > 0 ? 0.008f : controls.FeatureSpacing, phase, adaptive);
#if FIDGET_CURVED_SAMPLING
                        settings.SurfaceError = error;
#endif
                        using (var stream = new MeshStream(shape, settings))
                        {
                            values.Set("boreDiameter", controls.BoreDiameter);
                            values.Set("bridgeThickness", controls.BridgeThickness);
                            stream.TrySchedule(values);
                            var deadline = DateTime.UtcNow.AddSeconds(30);
                            while (!stream.TryApply())
                            {
                                if (!stream.IsPending || DateTime.UtcNow > deadline) throw new InvalidOperationException("Comparison build failed");
                                Thread.Sleep(1);
                            }
                            var mesh = stream.Mesh;
#if FIDGET_CURVED_SAMPLING
                            if (error > 0 && stream.LastCurvedLeaves == 0) throw new InvalidDataException("No curved patch was accepted");
#endif
                            if (mesh.normals.Length != mesh.vertexCount || !Covers(mesh, new Vector2(0.011f, 0.003f)) ||
                                !Covers(mesh, new Vector2(-0.48f, controls.BoreDiameter * 0.65f)) ||
                                Covers(mesh, new Vector2(-0.48f, 0.002f)))
                                throw new InvalidDataException($"Bridge, bore or normals failed at grid phase {phase}, adaptive={adaptive}");
                            Debug.Log($"[FidgetCoupon] phase={phase} adaptive={adaptive} error={error} visited={stream.LastVisitedCells} triangles={stream.TriangleCount}");
                        }
                    }
                }
            }
            finally { EditorSceneManager.ClosePreviewScene(scene); }
            return "PASS rounded block comparison at three grid phases";
#else
            throw new InvalidOperationException("Sampling comparison requires Fidget 0.8 or later.");
#endif
        }

        private static (bool adaptive, float error)[] Modes()
        {
#if FIDGET_CURVED_SAMPLING
            return new[] { (false, 0f), (true, 0f), (true, 0.008f) };
#else
            return new[] { (false, 0f), (true, 0f) };
#endif
        }

        // Orthographic ray through the XY position, independent of Unity physics.
        private static bool Covers(Mesh mesh, Vector2 point)
        {
            var vertices = mesh.vertices;
            var indices = mesh.triangles;
            float Side(Vector2 a, Vector2 b) => (b.x-a.x)*(point.y-a.y) - (b.y-a.y)*(point.x-a.x);
            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector2 a = vertices[indices[i]], b = vertices[indices[i+1]], c = vertices[indices[i+2]];
                float u = Side(a,b), v = Side(b,c), w = Side(c,a);
                if (Mathf.Abs(u+v+w) > 1e-12f && ((u >= 0 && v >= 0 && w >= 0) || (u <= 0 && v <= 0 && w <= 0))) return true;
            }
            return false;
        }

        public static void CheckScene() => Debug.Log($"[FidgetCoupon] {Validate()}");
    }
}
