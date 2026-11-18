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
            if (entry.Scene == "Transforms/Elongation")
            {
                // Outer contour is the sphere radius plus the XY stretch and stroke.
                var section = new Bounds(Vector3.zero, new Vector3(1.6f, 1.05f, 0.12f));
                ExampleMeshChecks.Configure(mesh, section, MeshNormals.Surface, 6);
                return section;
            }
            var property = entry.Property == null ? null : entry.Type.GetProperty(entry.Property);
            var original = property?.GetValue(mesh);
            var bounds = new Bounds();
            bool first = true;
            try
            {
                // These clipped XY curves fit inside a two-unit square; a shallow Z
                // domain avoids evaluating their expensive fields far from the stroke.
                bool curve = entry.Scene == "Profiles/Curves";
                var search = new Bounds(Vector3.zero, curve ? new Vector3(2.4f, 2.4f, 0.5f) : Vector3.one * 8);
                ExampleMeshChecks.Configure(mesh, search, MeshNormals.None, curve ? 5 : 7);
                var interior = search;
                interior.Expand(-0.03f);
                for (int i = -1; i <= (property == null ? -1 : 4); i++)
                {
                    var value = i < 0 ? original : Convert.ChangeType(Mathf.Lerp(entry.Minimum, entry.Maximum, i / 4f), property.PropertyType);
                    property?.SetValue(mesh, value);
                    var result = ExampleMeshChecks.Apply(mesh, false);
                    foreach (var vertex in result.vertices)
                    {
                        if (!interior.Contains(vertex))
                            throw new Exception(entry.Type.Name + " needs a larger initial fitting volume");
                        if (first) { bounds = new Bounds(vertex, Vector3.zero); first = false; }
                        else bounds.Encapsulate(vertex);
                    }
                }
            }
            finally { property?.SetValue(mesh, original); }
            bounds.Expand(bounds.size * 0.2f + Vector3.one * 0.08f);
            ExampleMeshChecks.Configure(mesh, bounds, MeshNormals.Surface, entry.Scene.Contains("ColumnJoins") || entry.Scene.Contains("StairJoins") ? 7 : 6);
            return bounds;
        }
    }
}
