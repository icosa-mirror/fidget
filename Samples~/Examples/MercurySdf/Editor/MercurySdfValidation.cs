using System;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Icosa.Fidget;

namespace FidgetExamples.Editor
{
    /// <summary>Editor-only integration probe for the sample, without entering Play mode.</summary>
    public static class MercurySdfValidation
    {
        public static string Run()
        {
            var script = AssetDatabase.FindAssets("MercurySdfValidation t:MonoScript")[0];
            var folder = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(
                AssetDatabase.GUIDToAssetPath(script)))).Replace('\\', '/');
            string path = folder + "/Scenes/MercurySdf.unity";
            var scene = EditorSceneManager.OpenPreviewScene(path);
            var counts = new int[6];
            try
            {
                Camera camera = null;
                foreach (var root in scene.GetRootGameObjects())
                {
                    foreach (var child in root.GetComponentsInChildren<Transform>()) child.gameObject.layer = 31;
                    if (root.TryGetComponent<Camera>(out var c)) camera = c;
                    if (root.TryGetComponent<Light>(out var light)) light.cullingMask = 1 << 31;
                }
                if (camera == null) throw new Exception("Sample setup incomplete");
                foreach (var root in scene.GetRootGameObjects())
                {
                    if (root.TryGetComponent<MercuryPrimitives>(out var primitives)) counts[0] = Validate(primitives, r => primitives.ConeRadius = r);
                    if (root.TryGetComponent<MercuryRound>(out var round)) counts[1] = Validate(round, r => round.Radius = r);
                    if (root.TryGetComponent<MercuryChamfer>(out var chamfer)) counts[2] = Validate(chamfer, r => chamfer.Radius = r);
                    if (root.TryGetComponent<MercuryColumns>(out var columns)) counts[3] = Validate(columns, r => columns.Radius = r);
                    if (root.TryGetComponent<MercuryStairs>(out var stairs)) counts[4] = Validate(stairs, r => stairs.Radius = r);
                    if (root.TryGetComponent<MercuryPolar>(out var polar)) counts[5] = Validate(polar, r => polar.Radius = r);
                }
                foreach (var count in counts) if (count == 0) throw new Exception("Missing sample component");
                camera.scene = scene;
                camera.cullingMask = 1 << 31;
                camera.aspect = 16f / 9f;
                Capture(camera);
                return "PASS: six Mercury meshes, surface normals, live parameter updates and preview; vertex counts " + string.Join(", ", counts);
            }
            finally
            {
                foreach (var root in scene.GetRootGameObjects())
                    foreach (var example in root.GetComponentsInChildren<FidgetMesh>())
                        UnityEngine.Object.DestroyImmediate(example); // Complete workers and release native resources.
                EditorSceneManager.ClosePreviewScene(scene);
            }
        }
        private static int Validate<T>(T example, Action<float> setRadius) where T : FidgetMesh<T>
        {
            example.Rebuild(); Apply(example);
            int count = example.Stream.VertexCount;
            var before = example.Stream.Mesh.vertices;
            setRadius(0.25f); Apply(example);
            var after = example.Stream.Mesh.vertices;
            bool changed = before.Length != after.Length;
            for (int i = 0; !changed && i < before.Length; ++i) changed = (before[i] - after[i]).sqrMagnitude > 0.000001f;
            if (!changed) throw new Exception("Parameter update did not change " + example.name);
            setRadius(example is MercuryPolar ? 0.12f : example is MercuryPrimitives ? 0.4f : 0.35f); Apply(example);
            foreach (var vertex in example.Stream.Mesh.vertices)
                if (!example.Bounds.Contains(vertex)) throw new Exception("Sample vertex outside bounds");
            return count;
        }
        private static void Apply(FidgetMesh example)
        {
            if (!example.Stream.TrySchedule(example.Parameters)) throw new Exception("Could not schedule sample");
            var deadline = DateTime.UtcNow.AddSeconds(30);
            while (!example.Stream.TryApply())
            {
                if (DateTime.UtcNow > deadline) throw new TimeoutException("Sample mesh job did not finish");
                Thread.Sleep(1);
            }
            if (example.Stream.VertexCount == 0 || example.Stream.TriangleCount == 0) throw new Exception("Empty sample mesh");
            var mesh = example.Stream.Mesh;
            if (mesh.normals.Length != mesh.vertexCount) throw new Exception("Missing surface normals");
            foreach (var vertex in mesh.vertices)
                if (float.IsNaN(vertex.x) || float.IsInfinity(vertex.x))
                    throw new Exception("Invalid sample vertex");
        }
        private static void Capture(Camera camera)
        {
            var texture = new RenderTexture(1280, 720, 24);
            var image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            var previous = RenderTexture.active;
            try
            {
                camera.targetTexture = texture;
                camera.Render();
                RenderTexture.active = texture;
                image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); image.Apply();
                Directory.CreateDirectory("../../target/mercury-checks");
                File.WriteAllBytes("../../target/mercury-checks/gallery.png", image.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = null; RenderTexture.active = previous;
                UnityEngine.Object.DestroyImmediate(texture); UnityEngine.Object.DestroyImmediate(image);
            }
        }
    }
}
