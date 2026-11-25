using System;
using System.Linq;
using System.Collections.Generic;
using Icosa.Fidget;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace FidgetExamples.Editor
{
    public static class ExampleValidation
    {
        private static void CheckBounds(string name, Vector3[] vertices, Bounds bounds)
        {
            bounds.Expand(-0.015f);
            foreach (var vertex in vertices)
                if (!bounds.Contains(vertex)) throw new Exception(name + " reaches its sampling boundary");
        }

        public static string Run(string name)
        {
            if (name == SamplingComparisonScene.Name) return SamplingComparisonScene.Validate();
            var entries = ExampleCatalog.Entries.Where(e => e.Scene == name).ToArray();
            if (entries.Length == 0) throw new ArgumentException(name);
            var scene = EditorSceneManager.OpenPreviewScene(ExampleSceneLayout.RootFolder() + "/Scenes/" + name + ".unity");
            int vertices = 0;
            try
            {
                foreach (var entry in entries)
                {
                    var mesh = scene.GetRootGameObjects().Select(g => g.GetComponent(entry.Type)).OfType<FidgetMesh>().Single();
                    entry.Type.GetMethod("Rebuild").Invoke(mesh, null);
                    var result = ExampleMeshChecks.Apply(mesh);
                    vertices += result.vertexCount;
                    var before = result.vertices;
                    var parameters = mesh.Parameters;
                    var bounds = (Bounds)entry.Type.GetField("Bounds").GetValue(mesh);
                    CheckBounds(entry.Type.Name, before, bounds);
                    var changed = new Dictionary<string, bool>();
                    ExampleControls.Sweep(mesh, control =>
                    {
                        var after = ExampleMeshChecks.Apply(mesh).vertices;
                        CheckBounds(entry.Type.Name + " at " + control, after, bounds);
                        if (!ReferenceEquals(parameters, mesh.Parameters)) throw new Exception("Property change rebuilt the graph");
                        bool different = before.Length != after.Length;
                        for (int j = 0; !different && j < before.Length; j++)
                            different = (before[j] - after[j]).sqrMagnitude > 0.000001f;
                        changed[control] = different || (changed.TryGetValue(control, out var earlier) && earlier);
                    });
                    foreach (var control in changed)
                        if (!control.Value && control.Key != "combined sliders") throw new Exception(entry.Type.Name + " ignored " + control.Key);
                    ExampleMeshChecks.Apply(mesh);
                }
                ExampleRender.Capture(scene, name);
                return "PASS " + name + ": " + entries.Length + " meshes; " + vertices + " vertices; finite normals, parameter sweep and bounds";
            }
            finally
            {
                foreach (var mesh in scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<FidgetMesh>())) UnityEngine.Object.DestroyImmediate(mesh);
                EditorSceneManager.ClosePreviewScene(scene);
            }
        }
    }
}
