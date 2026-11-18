using System.IO;
using Icosa.Fidget;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
namespace FidgetExamples.Editor
{
    internal static class ExampleSceneLayout
    {
        internal static string RootFolder()
        {
            var script = AssetDatabase.FindAssets("ExampleSceneLayout t:MonoScript")[0];
            return Path.GetDirectoryName(Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(script))).Replace('\\', '/');
        }
        internal static void Folder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parent = Path.GetDirectoryName(path).Replace('\\', '/');
            Folder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
        internal static void Environment(int count)
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.45f, 0.48f, 0.55f);
            var camera = new GameObject("Main Camera", typeof(Camera)).GetComponent<Camera>();
            camera.tag = "MainCamera";
            camera.transform.position = new Vector3(0, 0, -20);
            camera.orthographic = true;
            camera.orthographicSize = count > 3 ? 2.8f : count == 3 ? 2.2f : 1.8f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.035f, 0.05f, 0.08f);
            var light = new GameObject("Key Light", typeof(Light)).GetComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.8f;
            light.transform.rotation = Quaternion.Euler(35, -30, 0);
        }
        internal static void Place(FidgetMesh mesh, Bounds bounds, ExampleCatalog.Entry entry, int index, int count)
        {
            int columns = Mathf.Min(3, count), rows = Mathf.CeilToInt(count / (float)columns);
            var location = new Vector3((index % columns - (columns - 1) * 0.5f) * 2.4f,
                ((rows - 1) * 0.5f - index / columns) * 2.4f + 0.15f, 0);
            var rotation = Quaternion.Euler(entry.Profile ? 12 : 22, -15, 0);
            float scale = 1.6f / Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            mesh.transform.localScale = Vector3.one * scale;
            mesh.transform.rotation = rotation;
            mesh.transform.position = location - rotation * (bounds.center * scale);
            mesh.GetComponent<MeshRenderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(RootFolder() +
                (index % 2 == 0 ? "/Rendering/Fidget Orange.mat" : "/Rendering/Fidget Blue.mat"));
            var label = new GameObject(entry.Title + " label", typeof(TextMesh)).GetComponent<TextMesh>();
            label.transform.position = location + new Vector3(0, -1.05f, -0.7f);
            label.text = entry.Title;
            label.anchor = TextAnchor.MiddleCenter;
            label.fontSize = 40;
            label.characterSize = 0.035f;
            label.color = Color.white;
        }
    }
}
