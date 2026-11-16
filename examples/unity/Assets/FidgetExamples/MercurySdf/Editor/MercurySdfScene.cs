using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace FidgetExamples.Editor
{
    /// <summary>Recreates the sample through Unity's scene API, preserving the caller's scene.</summary>
    public static class MercurySdfScene
    {
        private static void Add<T>(GameObject go) where T : Icosa.Fidget.FidgetMesh<T>
        {
            var example = go.AddComponent<T>();
            example.Bounds = new Bounds(Vector3.zero, new Vector3(3.2f, 3.2f, 3.2f));
            example.Detail = 6; example.Continuous = false;
        }

        [MenuItem("Fidget/Create Mercury SDF Sample")]
        public static void Create()
        {
            var script = AssetDatabase.FindAssets("MercurySdfScene t:MonoScript")[0];
            var folder = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(
                System.IO.Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(script)))).Replace('\\', '/');
            string path = folder + "/Scenes/MercurySdf.unity";
            var previous = SceneManager.GetActiveScene();
            bool replacingOpenScene = previous.path == path;
            if (replacingOpenScene && previous.isDirty)
                throw new System.InvalidOperationException("Save or close the modified MercurySdf scene before recreating it.");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            if (replacingOpenScene) EditorSceneManager.CloseScene(previous, true);
            try
            {
                RenderSettings.ambientMode = AmbientMode.Flat;
                RenderSettings.ambientLight = new Color(0.45f, 0.48f, 0.55f);
                var camera = new GameObject("Main Camera", typeof(Camera)).GetComponent<Camera>();
                camera.tag = "MainCamera";
                camera.transform.position = new Vector3(-0.5f, 0, -10);
                camera.orthographic = true; camera.orthographicSize = 3.6f;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.035f, 0.05f, 0.08f);
                var light = new GameObject("Key Light", typeof(Light)).GetComponent<Light>();
                light.type = LightType.Directional; light.intensity = 1.8f;
                light.transform.rotation = Quaternion.Euler(35, -30, 0);
                string[] titles = {"Cones and capsules", "Rounded union", "Chamfered union", "Columns union", "Stairs union", "Polar repetition"};
                for (int i = 0; i < titles.Length; ++i)
                {
                    var go = new GameObject(titles[i]);
                    go.transform.position = new Vector3((i % 3 - 1) * 3.2f, i < 3 ? 1.35f : -1.35f, 0);
                    go.transform.rotation = Quaternion.Euler(i == 5 ? 35 : 15, -15, 0);
                    if (i == 0) Add<MercuryPrimitives>(go);
                    if (i == 1) Add<MercuryRound>(go);
                    if (i == 2) Add<MercuryChamfer>(go);
                    if (i == 3) Add<MercuryColumns>(go);
                    if (i == 4) Add<MercuryStairs>(go);
                    if (i == 5) Add<MercuryPolar>(go);
                    go.GetComponent<MeshRenderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(folder + (i % 2 == 0 ? "/Rendering/Fidget Orange.mat" : "/Rendering/Fidget Blue.mat"));
                    var label = new GameObject("Label", typeof(TextMesh)).GetComponent<TextMesh>();
                    label.transform.position = go.transform.position + new Vector3(0, -0.95f, 0);
                    label.text = titles[i]; label.anchor = TextAnchor.MiddleCenter;
                    label.fontSize = 40; label.characterSize = 0.035f; label.color = Color.white;
                }
                camera.transform.position = new Vector3(0, 0, -10);
                if (!EditorSceneManager.SaveScene(scene, path)) throw new System.IO.IOException("Could not save " + path);
                Debug.Log("Saved " + path);
            }
            finally
            {
                if (!replacingOpenScene)
                {
                    if (previous.IsValid()) SceneManager.SetActiveScene(previous);
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }
    }
}
