using System;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Icosa.Fidget.Editor
{
    public static class IntegrationCheck
    {
        // Batch entry point. Run only in a dedicated validation project.
        public static void Run()
        {
            using (var stream = new SphereMeshStream())
            {
                Apply(stream, 0.6f, 4);
                int firstCount = stream.VertexCount;
                Check(stream.Mesh.bounds.size.x > 1.1f, "First bounds are too small");
                Apply(stream, 0.35f, 5);
                Check(stream.Mesh.bounds.size.x < 0.8f, "Bounds did not shrink after replacement");
                Check(stream.VertexCount != firstCount, "Expected topology to change");
                using (var read = Mesh.AcquireReadOnlyMeshData(stream.Mesh))
                {
                    var vertices = read[0].GetVertexData<Vector3>();
                    var indices = read[0].GetIndexData<uint>();
                    Check(indices.Length == stream.TriangleCount * 3, "Index count mismatch");
                    for (int i = 0; i < indices.Length; i++)
                        Check(indices[i] < vertices.Length, "Out of range index");
                    for (int i = 0; i < vertices.Length; i++)
                        Check(Mathf.Abs(vertices[i].magnitude - 0.35f) < 0.05f, "Unexpected vertex position");
                }
                RenderCheck(stream.Mesh);
                // Teardown must also safely finish and discard an unpublished update.
                Check(stream.Schedule(0.5f, 4), "Could not schedule teardown probe");
                Debug.Log($"[FidgetUnity] PASS: worker meshing, changing topology, bounds, indices, rendering; {stream.VertexCount} vertices, {stream.TriangleCount} triangles.");
            }
        }

        private static void Apply(SphereMeshStream stream, float radius, int depth)
        {
            Check(stream.Schedule(radius, depth), "Could not schedule mesh");
            Check(!stream.Schedule(radius, depth), "A second job should not be queued");
            var deadline = DateTime.UtcNow.AddSeconds(30);
            while (!stream.TryApply())
            {
                if (DateTime.UtcNow > deadline) throw new TimeoutException("Fidget worker did not finish");
                Thread.Sleep(1);
            }
            Check(stream.VertexCount > 0 && stream.TriangleCount > 0, "Empty sphere");
        }

        private static void RenderCheck(Mesh mesh)
        {
            var shader = Shader.Find("Fidget/Preview");
            Check(shader != null && shader.isSupported, "Preview shader unavailable");
            var material = new Material(shader);
            var shape = new GameObject("Fidget render probe", typeof(MeshFilter), typeof(MeshRenderer));
            var cameraObject = new GameObject("Fidget camera probe", typeof(Camera));
            var texture = new RenderTexture(96, 96, 24);
            var pixels = new Texture2D(96, 96, TextureFormat.RGB24, false);
            var previous = RenderTexture.active;
            try
            {
                shape.GetComponent<MeshFilter>().sharedMesh = mesh;
                shape.GetComponent<MeshRenderer>().sharedMaterial = material;
                var camera = cameraObject.GetComponent<Camera>();
                camera.transform.position = new Vector3(0, 0, -1.5f);
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.black;
                camera.targetTexture = texture;
                camera.Render();
                RenderTexture.active = texture;
                pixels.ReadPixels(new Rect(0, 0, 96, 96), 0, 0);
                pixels.Apply();
                var center = pixels.GetPixel(48, 48);
                Check(center.g > 0.05f && center.b > 0.05f, "Sphere was not visible at image center");
                Directory.CreateDirectory("Logs");
                File.WriteAllBytes("Logs/fidget-preview.png", pixels.EncodeToPNG());
            }
            finally
            {
                RenderTexture.active = previous;
                UnityEngine.Object.DestroyImmediate(cameraObject);
                UnityEngine.Object.DestroyImmediate(shape);
                UnityEngine.Object.DestroyImmediate(material);
                UnityEngine.Object.DestroyImmediate(pixels);
                UnityEngine.Object.DestroyImmediate(texture);
            }
        }

        private static void Check(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException($"[FidgetUnity] {message}");
        }
    }
}
