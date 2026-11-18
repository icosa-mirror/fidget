using System;
using System.Threading;
using Icosa.Fidget;
using UnityEditor;
using UnityEngine;
namespace FidgetExamples.Editor
{
    internal static class ExampleMeshChecks
    {
        internal static void Configure(FidgetMesh mesh, Bounds bounds, MeshNormals normals, int detail)
        {
            mesh.Detail = detail;
            mesh.Continuous = false;
            var serialized = new SerializedObject(mesh);
            serialized.FindProperty("Bounds").boundsValue = bounds;
            serialized.FindProperty("Normals").enumValueIndex = (int)normals;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            mesh.GetType().GetMethod("Rebuild").Invoke(mesh, null);
        }
        internal static Mesh Apply(FidgetMesh mesh, bool normals = true)
        {
            if (!mesh.Stream.TrySchedule(mesh.Parameters)) throw new Exception("Could not schedule " + mesh.name);
            var deadline = DateTime.UtcNow.AddSeconds(45);
            while (!mesh.Stream.TryApply())
            {
                if (DateTime.UtcNow > deadline) throw new TimeoutException(mesh.name);
                Thread.Sleep(1);
            }
            var result = mesh.Stream.Mesh;
            if (result.vertexCount == 0 || result.triangles.Length == 0) throw new Exception("Empty mesh: " + mesh.name);
            foreach (var p in result.vertices)
                if (!float.IsFinite(p.x) || !float.IsFinite(p.y) || !float.IsFinite(p.z)) throw new Exception("Invalid vertex: " + mesh.name);
            if (normals)
            {
                if (result.normals.Length != result.vertexCount) throw new Exception("Missing normals: " + mesh.name);
                foreach (var n in result.normals)
                    if (!float.IsFinite(n.x) || !float.IsFinite(n.y) || !float.IsFinite(n.z) || n.sqrMagnitude < 0.5f)
                        throw new Exception("Invalid normal: " + mesh.name);
            }
            return result;
        }
    }
}
