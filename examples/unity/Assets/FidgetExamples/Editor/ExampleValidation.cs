using System;
using System.Linq;
using Icosa.Fidget;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace FidgetExamples.Editor
{
    public static class ExampleValidation
    {
        public static string Run(string name)
        {
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
                    if (entry.Property == null) continue; // Fixed motifs have no shape parameter.
                    var property = entry.Type.GetProperty(entry.Property);
                    var original = property.GetValue(mesh);
                    var bounds = (Bounds)entry.Type.GetField("Bounds").GetValue(mesh);
                    bool changed = false;
                    try
                    {
                        for (int i = 0; i <= 4; i++)
                        {
                            property.SetValue(mesh, Convert.ChangeType(Mathf.Lerp(entry.Minimum, entry.Maximum, i / 4f), property.PropertyType));
                            result = ExampleMeshChecks.Apply(mesh);
                            var after = result.vertices;
                            changed |= before.Length != after.Length;
                            for (int j = 0; !changed && j < before.Length; j++) changed = (before[j] - after[j]).sqrMagnitude > 0.000001f;
                            var interior = bounds; interior.Expand(-0.015f);
                            foreach (var vertex in after)
                                if (!interior.Contains(vertex)) throw new Exception(entry.Type.Name + " reaches its sampling boundary");
                            if (!ReferenceEquals(parameters, mesh.Parameters)) throw new Exception("Property change rebuilt the graph");
                        }
                    }
                    finally { property.SetValue(mesh, original); }
                    if (!changed) throw new Exception(entry.Type.Name + " ignored " + entry.Property);
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
