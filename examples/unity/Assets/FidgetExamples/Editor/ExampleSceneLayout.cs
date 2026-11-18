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
            if (entry.Type == typeof(SphereCavityExample)) rotation = Quaternion.Euler(0, 60, 0);
            if (entry.Type == typeof(CutHollowSphereExample)) rotation = Quaternion.Euler(-55, 0, 0);
            if (entry.Scene == "Repetition/Polar") rotation = Quaternion.Euler(-43, 12, 0);
            if (entry.Scene == "Construction/ProfilesToSolids") rotation = Quaternion.Euler(-20, 45, 0);
            if (entry.Scene == "Transforms/Elongation") rotation = Quaternion.identity;
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
            var material = mesh.GetComponent<MeshRenderer>().sharedMaterial;
            if (mesh is OctahedronBoundExample octahedron)
            {
                var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                marker.name = "Distance probe";
                Object.DestroyImmediate(marker.GetComponent<Collider>());
                marker.transform.SetParent(mesh.transform, false);
                marker.transform.localScale = Vector3.one * 0.08f;
                marker.GetComponent<MeshRenderer>().sharedMaterial = material;
                var probe = mesh.gameObject.AddComponent<OctahedronProbe>();
                probe.Target = octahedron;
                probe.Marker = marker.transform;
                probe.Readout = Object.Instantiate(label);
                probe.Readout.transform.position += Vector3.down * 0.22f;
                probe.Readout.characterSize *= 0.8f;
                probe.Refresh();
            }
            if (mesh is PolarYExample || mesh is PolarZExample)
            {
                var line = Line("Rotation axis", mesh.transform, material);
                Vector3 axis = mesh is PolarYExample ? Vector3.up : Vector3.forward;
                line.SetPositions(new[] { -axis * 0.7f, axis * 0.7f });
            }
            if (entry.Scene == "Construction/ProfilesToSolids")
            {
                mesh.transform.position += Vector3.up * 0.25f;
                var center = location + Vector3.down * 0.74f;
                if (mesh is LoftExample loft)
                {
                    Outline(center + Vector3.left * 0.32f, material, new Vector2(0.4f, 0.4f), 0, true);
                    Outline(center + Vector3.right * 0.32f, material, new Vector2(loft.EndWidth, 0.22f), 0.06f, false, loft);
                }
                else if (mesh is RevolutionExample) Outline(center, material, Vector2.one * 0.2f, 0, true);
                else Outline(center, material, new Vector2(0.4f, 0.3f), 0.1f, false);
            }
        }

        private static LineRenderer Line(string name, Transform parent, Material material)
        {
            var line = new GameObject(name, typeof(LineRenderer)).GetComponent<LineRenderer>();
            line.transform.SetParent(parent, false);
            line.useWorldSpace = false;
            line.sharedMaterial = material;
            line.widthMultiplier = 0.018f;
            line.generateLightingData = true;
            line.positionCount = 2;
            return line;
        }

        private static void Outline(Vector3 center, Material material, Vector2 size, float radius, bool circle, LoftExample loft = null)
        {
            var line = Line("Source profile", null, material);
            line.transform.position = center;
            line.transform.localScale = Vector3.one * 0.62f;
            line.loop = true;
            var outline = line.gameObject.AddComponent<ProfileOutline>();
            outline.Line = line;
            outline.Loft = loft;
            outline.HalfSize = size;
            outline.CornerRadius = radius;
            outline.Circle = circle;
            outline.Refresh();
        }
    }
}
