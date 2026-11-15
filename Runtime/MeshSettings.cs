using System;
using UnityEngine;

namespace Icosa.Fidget
{
    public enum MeshNormals { None, Smooth, Surface }

    /// <summary>Local-space domain, octree depth, and optional vertex normals.</summary>
    [Serializable]
    public struct MeshSettings
    {
        public Bounds Bounds;
        [Range(1, 8)] public int Depth;
        public MeshNormals Normals;
        public MeshSettings(Bounds bounds, int depth = 5, MeshNormals normals = MeshNormals.None)
        { Bounds = bounds; Depth = depth; Normals = normals; }
        internal static void ValidateDepth(int depth)
        {
            if (depth < 1 || depth > 8) throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be between 1 and 8.");
        }
        internal void Validate()
        {
            ValidateDepth(Depth);
            if (Normals != MeshNormals.None && Normals != MeshNormals.Smooth && Normals != MeshNormals.Surface) throw new ArgumentOutOfRangeException(nameof(Normals));
            for (int i = 0; i < 3; ++i)
            {
                ShapeBuilder.Finite(Bounds.min[i], nameof(Bounds));
                ShapeBuilder.Finite(Bounds.max[i], nameof(Bounds));
                ShapeBuilder.Finite(Bounds.size[i], nameof(Bounds));
                if (Bounds.size[i] <= 0) throw new ArgumentException("Meshing bounds must have positive size on every axis.", nameof(Bounds));
            }
        }
    }
}
