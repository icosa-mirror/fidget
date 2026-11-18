using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Icosa.Fidget;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace FidgetExamples.Editor
{
    public static class SdfGalleryValidation
    {
        public static string Run(string sceneName, string onlyType = null)
        {
            var page = SdfGalleryCatalog.Pages.Single(p => p.Scene == sceneName);
            var scene = EditorSceneManager.OpenPreviewScene(SdfGalleryScene.RootFolder() + "/Scenes/" + sceneName + ".unity");
            int totalVertices = 0;
            try
            {
                foreach (var entry in page.Entries.Where(e => onlyType == null || e.Type.Name == onlyType))
                {
                    Debug.Log("[FidgetGallery] Checking " + entry.Type.Name);
                    var mesh = scene.GetRootGameObjects().Select(g => g.GetComponent(entry.Type)).OfType<FidgetMesh>().Single();
                    foreach (var t in mesh.GetComponentsInChildren<Transform>()) t.gameObject.layer = 31;
                    var rebuild = entry.Type.GetMethod("Rebuild"); rebuild.Invoke(mesh, null);
                    Apply(mesh);
                    totalVertices += mesh.Stream.VertexCount;
                    var before = mesh.Stream.Mesh.vertices;
                    var graphParameters = mesh.Parameters;
                    var property = entry.Type.GetProperty(entry.Property);
                    float original = Convert.ToSingle(property.GetValue(mesh));
                    try
                    {
                        property.SetValue(mesh, Convert.ChangeType(entry.Integer ? original + 1 : original * 1.15f, property.PropertyType));
                        Apply(mesh);
                        var after = mesh.Stream.Mesh.vertices;
                        bool changed = before.Length != after.Length;
                        for (int i = 0; !changed && i < before.Length; i++) changed = (before[i] - after[i]).sqrMagnitude > 0.000001f;
                        if (!changed) throw new Exception(entry.Type.Name + " ignored " + entry.Property);
                        if (!ReferenceEquals(graphParameters, mesh.Parameters)) throw new Exception("Parameter change rebuilt the graph");
                    }
                    finally { property.SetValue(mesh, Convert.ChangeType(original, property.PropertyType)); }
                    Apply(mesh);
                    var bounds = (Bounds)entry.Type.GetField("Bounds").GetValue(mesh);
                    foreach (var vertex in mesh.Stream.Mesh.vertices)
                        if (!bounds.Contains(vertex)) throw new Exception(entry.Type.Name + " exceeded its sampling bounds");
                    Debug.Log("[FidgetGallery] PASS " + entry.Type.Name);
                }
                var camera = scene.GetRootGameObjects().Select(go => go.GetComponent<Camera>()).First(c => c != null);
                camera.scene = scene; camera.cullingMask = 1 << 31; camera.aspect = 16f / 10f;
                foreach (var light in scene.GetRootGameObjects().Select(go => go.GetComponent<Light>()).Where(l => l != null)) light.cullingMask = 1 << 31;
                foreach (var label in scene.GetRootGameObjects().Select(go => go.GetComponent<TextMesh>()).Where(t => t != null)) label.gameObject.layer = 31;
                Capture(camera, sceneName);
                return "PASS: " + sceneName + ": " + page.Entries.Length + " meshes, normals, parameter updates without graph rebuilds; " + totalVertices + " vertices";
            }
            finally
            {
                foreach (var mesh in scene.GetRootGameObjects().SelectMany(go => go.GetComponentsInChildren<FidgetMesh>()))
                    UnityEngine.Object.DestroyImmediate(mesh);
                EditorSceneManager.ClosePreviewScene(scene);
            }
        }
        private static void Apply(FidgetMesh mesh)
        {
            try { ApplyCore(mesh); }
            catch (Exception error) { throw new Exception(mesh.name + " mesh validation failed: " + error.Message, error); }
        }
        private static void ApplyCore(FidgetMesh mesh)
        {
            // Typed bindings are captured by TrySchedule; no graph rebuild is required.
            if (!mesh.Stream.TrySchedule(mesh.Parameters)) throw new Exception("Could not schedule " + mesh.name);
            var deadline = DateTime.UtcNow.AddSeconds(45);
            while (!mesh.Stream.TryApply())
            {
                if (DateTime.UtcNow > deadline) throw new TimeoutException(mesh.name);
                Thread.Sleep(1);
            }
            var result = mesh.Stream.Mesh;
            if (result.vertexCount == 0 || result.triangles.Length == 0) throw new Exception("Empty mesh: " + mesh.name);
            if (result.normals.Length != result.vertexCount) throw new Exception("Missing normals: " + mesh.name);
            foreach (var p in result.vertices)
                if (!float.IsFinite(p.x) || !float.IsFinite(p.y) || !float.IsFinite(p.z)) throw new Exception("Invalid vertex: " + mesh.name);
        }
        private static void Capture(Camera camera, string name)
        {
            var target = new RenderTexture(2048, 1280, 24);
            var pixels = new Texture2D(2048, 1280, TextureFormat.RGB24, false);
            var previous = RenderTexture.active;
            try
            {
                camera.targetTexture = target; camera.Render(); RenderTexture.active = target;
                pixels.ReadPixels(new Rect(0, 0, 2048, 1280), 0, 0); pixels.Apply();
                Directory.CreateDirectory("../../target/sdf-gallery-checks");
                File.WriteAllBytes("../../target/sdf-gallery-checks/" + name + ".png", pixels.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = null; RenderTexture.active = previous;
                UnityEngine.Object.DestroyImmediate(target); UnityEngine.Object.DestroyImmediate(pixels);
            }
        }
    }
}
