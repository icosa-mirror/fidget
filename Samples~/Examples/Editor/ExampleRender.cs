using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace FidgetExamples.Editor
{
    internal static class ExampleRender
    {
        internal static void Capture(Scene scene, string name)
        {
            foreach (var root in scene.GetRootGameObjects())
                foreach (var t in root.GetComponentsInChildren<Transform>()) t.gameObject.layer = 31;
            foreach (var light in scene.GetRootGameObjects().Select(g => g.GetComponent<Light>()).Where(l => l != null)) light.cullingMask = 1 << 31;
            var camera = scene.GetRootGameObjects().Select(g => g.GetComponent<Camera>()).First(c => c != null);
            camera.scene = scene; camera.cullingMask = 1 << 31; camera.aspect = 16f / 10f;
            var target = new RenderTexture(1600, 1000, 24);
            var pixels = new Texture2D(1600, 1000, TextureFormat.RGB24, false);
            var previous = RenderTexture.active;
            try
            {
                camera.targetTexture = target; camera.Render(); RenderTexture.active = target;
                pixels.ReadPixels(new Rect(0, 0, 1600, 1000), 0, 0); pixels.Apply();
                var path = "../../target/example-checks/" + name + ".png";
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllBytes(path, pixels.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = null; RenderTexture.active = previous;
                Object.DestroyImmediate(target); Object.DestroyImmediate(pixels);
            }
        }
    }
}
