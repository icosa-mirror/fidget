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
                var section = new Bounds(Vector3.zero, new Vector3(2.2f, 1.05f, 0.12f));
                ExampleMeshChecks.Configure(mesh, section, MeshNormals.Surface, 6);
                return section;
            }
            bool curve = entry.Scene == "Profiles/Curves";
            var search = new Bounds(Vector3.zero, curve ? new Vector3(2.8f, 2.8f, 1f) : Vector3.one * 6);
            // At maximum count and spacing the outer interval beads sit at X = ±3.4.
            if (mesh is RepeatIntervalXExample) search.size = new Vector3(7.6f, 0.6f, 0.6f);
            ExampleMeshChecks.Configure(mesh, search, MeshNormals.None, curve ? 5 : 7);
            var interior = search;
            interior.Expand(-0.03f);
            var bounds = new Bounds();
            bool first = true;
            void Include(string control)
            {
                var result = ExampleMeshChecks.Apply(mesh, false);
                foreach (var vertex in result.vertices)
                {
                    if (!interior.Contains(vertex))
                        throw new Exception($"{entry.Type.Name} needs a larger fitting volume at {control}");
                    if (first) { bounds = new Bounds(vertex, Vector3.zero); first = false; }
                    else bounds.Encapsulate(vertex);
                }
            }
            Include("default");
            ExampleControls.Sweep(mesh, Include);
            bounds.Expand(bounds.size * 0.2f + Vector3.one * 0.08f);
            ExampleMeshChecks.Configure(mesh, bounds, MeshNormals.Surface, entry.Scene.Contains("ColumnJoins") || entry.Scene.Contains("StairJoins") ? 7 : 6);
            return bounds;
        }
    }
}
