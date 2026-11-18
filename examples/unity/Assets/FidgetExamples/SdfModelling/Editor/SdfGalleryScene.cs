using System;
using System.IO;
using Icosa.Fidget;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace FidgetExamples.Editor
{
    /// <summary>Creates all three pages through Unity's scene API without changing the caller's open scene.</summary>
    public static class SdfGalleryScene
    {
        internal static string RootFolder()
        {
            var script = AssetDatabase.FindAssets("SdfGalleryScene t:MonoScript")[0];
            return Path.GetDirectoryName(Path.GetDirectoryName(
                Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(script)))).Replace('\\', '/');
        }
        [MenuItem("Fidget/Create Expanded SDF Samples")]
        public static string Create()
        {
            string folder = RootFolder();
            var previous = SceneManager.GetActiveScene();
            foreach (var page in SdfGalleryCatalog.Pages)
            {
                string path = folder + "/Scenes/" + page.Scene + ".unity";
                for (int i = 0; i < SceneManager.sceneCount; i++)
                    if (SceneManager.GetSceneAt(i).path == path)
                        throw new InvalidOperationException("Close " + page.Scene + " before recreating it.");
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                SceneManager.SetActiveScene(scene);
                try
                {
                    RenderSettings.ambientMode = AmbientMode.Flat;
                    RenderSettings.ambientLight = new Color(0.45f, 0.48f, 0.55f);
                    var camera = new GameObject("Main Camera", typeof(Camera)).GetComponent<Camera>();
                    camera.tag = "MainCamera"; camera.transform.position = new Vector3(0, 0, -20);
                    camera.orthographic = true; camera.orthographicSize = 7.1f;
                    camera.clearFlags = CameraClearFlags.SolidColor;
                    camera.backgroundColor = new Color(0.035f, 0.05f, 0.08f);
                    var light = new GameObject("Key Light", typeof(Light)).GetComponent<Light>();
                    light.type = LightType.Directional; light.intensity = 1.8f;
                    light.transform.rotation = Quaternion.Euler(35, -30, 0);
                    for (int i = 0; i < page.Entries.Length; i++)
                    {
                        var entry = page.Entries[i];
                        var go = new GameObject(entry.Title);
                        var mesh = (FidgetMesh)go.AddComponent(entry.Type);
                        var bounds = entry.Type == typeof(PolyhedronSolids) ? 4.2f : 3.4f;
                        mesh.Detail = entry.Type == typeof(ColumnJoins) || entry.Type == typeof(StairJoins) || entry.Type == typeof(LatticeRepetition) ? 7 : 6;
                        mesh.Continuous = false;
                        // All gallery entries use the typed base with these serialized settings.
                        var serialized = new SerializedObject(mesh);
                        serialized.FindProperty("Bounds").boundsValue = new Bounds(Vector3.zero, Vector3.one * bounds);
                        serialized.FindProperty("Normals").enumValueIndex = (int)MeshNormals.Surface;
                        serialized.ApplyModifiedPropertiesWithoutUndo();
                        go.transform.position = new Vector3((i % 4 - 1.5f) * 4.4f, (1 - i / 4) * 4.4f + 0.4f, 0);
                        go.transform.rotation = Quaternion.Euler(page.Scene == "SdfProfiles" ? 12 : 22, -15, 0);
                        go.GetComponent<MeshRenderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(folder +
                            (i % 2 == 0 ? "/Rendering/Fidget Orange.mat" : "/Rendering/Fidget Blue.mat"));
                        var label = new GameObject("Label", typeof(TextMesh)).GetComponent<TextMesh>();
                        label.transform.position = go.transform.position + new Vector3(0, -1.55f, -0.5f);
                        label.text = entry.Title; label.anchor = TextAnchor.MiddleCenter;
                        label.fontSize = 40; label.characterSize = 0.042f; label.color = Color.white;
                    }
                    if (!EditorSceneManager.SaveScene(scene, path)) throw new IOException("Could not save " + path);
                }
                finally
                {
                    if (previous.IsValid()) SceneManager.SetActiveScene(previous);
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
            return "Created SdfSolids, SdfProfiles and SdfOperations (34 focused components)";
        }
    }
}
