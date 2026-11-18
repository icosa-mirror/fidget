using System;
using System.IO;
using System.Linq;
using Icosa.Fidget;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace FidgetExamples.Editor
{
    public static class ExampleScenes
    {
        public static string[] Names() => ExampleCatalog.Entries.Select(e => e.Scene).Distinct().OrderBy(s => s).ToArray();
        [MenuItem("Fidget/Create Feature Examples")]
        public static void CreateAll() { foreach (var name in Names()) Debug.Log(Create(name)); }
        public static string Create(string name)
        {
            var entries = ExampleCatalog.Entries.Where(e => e.Scene == name).ToArray();
            if (entries.Length == 0) throw new ArgumentException("Unknown example scene " + name);
            string path = ExampleSceneLayout.RootFolder() + "/Scenes/" + name + ".unity";
            for (int i = 0; i < SceneManager.sceneCount; i++)
                if (SceneManager.GetSceneAt(i).path == path) throw new InvalidOperationException("Close " + name + " before recreating it");
            ExampleSceneLayout.Folder(Path.GetDirectoryName(path).Replace('\\', '/'));
            var previous = SceneManager.GetActiveScene();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            try
            {
                ExampleSceneLayout.Environment(entries.Length);
                for (int i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];
                    var go = new GameObject(entry.Title);
                    var mesh = (FidgetMesh)go.AddComponent(entry.Type);
                    var bounds = ExampleBounds.Fit(mesh, entry);
                    ExampleSceneLayout.Place(mesh, bounds, entry, i, entries.Length);
                    // Native meshes are runtime resources, never serialized into scene assets.
                    go.GetComponent<MeshFilter>().sharedMesh = null;
                }
                if (!EditorSceneManager.SaveScene(scene, path)) throw new IOException("Could not save " + path);
            }
            finally
            {
                foreach (var mesh in scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<FidgetMesh>())) UnityEngine.Object.DestroyImmediate(mesh);
                if (previous.IsValid()) SceneManager.SetActiveScene(previous);
                EditorSceneManager.CloseScene(scene, true);
            }
            return "Created " + name + ": " + entries.Length + " independently bounded meshes";
        }
    }
}
