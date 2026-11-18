using System;
using Icosa.Fidget;
using UnityEngine;
namespace FidgetExamples.Editor
{
    internal static class ExampleBounds
    {
        // Fit the local sampling volume before saving; display placement never enters the SDF.
        internal static Bounds Fit(FidgetMesh mesh, ExampleCatalog.Entry entry)
        {
            var property = entry.Type.GetProperty(entry.Property);
            var original = property.GetValue(mesh);
            var bounds = new Bounds();
            bool first = true;
            try
            {
                ExampleMeshChecks.Configure(mesh, new Bounds(Vector3.zero, Vector3.one * 8), MeshNormals.None, 7);
                for (int i = -1; i <= 4; i++)
                {
                    var value = i < 0 ? original : Convert.ChangeType(Mathf.Lerp(entry.Minimum, entry.Maximum, i / 4f), property.PropertyType);
                    property.SetValue(mesh, value);
                    var result = ExampleMeshChecks.Apply(mesh, false);
                    foreach (var vertex in result.vertices)
                    {
                        if (Mathf.Max(Mathf.Abs(vertex.x), Mathf.Abs(vertex.y), Mathf.Abs(vertex.z)) > 3.9f)
                            throw new Exception(entry.Type.Name + " needs a larger initial fitting volume");
                        if (first) { bounds = new Bounds(vertex, Vector3.zero); first = false; }
                        else bounds.Encapsulate(vertex);
                    }
                }
            }
            finally { property.SetValue(mesh, original); }
            bounds.Expand(bounds.size * 0.2f + Vector3.one * 0.08f);
            ExampleMeshChecks.Configure(mesh, bounds, MeshNormals.Surface, entry.Scene.Contains("ColumnJoins") || entry.Scene.Contains("StairJoins") ? 7 : 6);
            return bounds;
        }
    }
}
