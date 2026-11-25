using System;
using UnityEngine;

namespace Icosa.Fidget
{
    public enum MeshNormals { None, Smooth, Surface }

    /// <summary>Local-space domain, octree depth, and optional vertex normals.</summary>
    [Serializable]
    public struct MeshSettings
    {
        public const int MaxDepth = 11;
        public const int DefaultCellBudget = 1000000;
        public Bounds Bounds;
        [Range(1, MaxDepth)] public int Depth;
        public MeshNormals Normals;
        [Min(0)] public float FeatureSpacing;
        [Min(0)] public int CellBudget;
        public bool AdaptivePlanes;
        public MeshSettings(Bounds bounds, int depth, MeshNormals normals)
            : this(bounds, depth, normals, 0, 0, false) { }
        public MeshSettings(Bounds bounds, int depth = 6, MeshNormals normals = MeshNormals.None,
            float featureSpacing = 0, int cellBudget = 0, bool adaptivePlanes = false)
        { Bounds = bounds; Depth = depth; Normals = normals; FeatureSpacing = featureSpacing; CellBudget = cellBudget; AdaptivePlanes = adaptivePlanes; }

        /// <summary>Depth of the finest sampling cells, capped at MaxDepth.</summary>
        public int SamplingDepth
        {
            get
            {
                ShapeBuilder.Finite(FeatureSpacing, nameof(FeatureSpacing));
                if (FeatureSpacing < 0) throw new ArgumentOutOfRangeException(nameof(FeatureSpacing));
                if (FeatureSpacing == 0) { ValidateDepth(Depth); return Depth; }
                int depth = 1;
                while (depth < MaxDepth && LongestCellEdge(depth) > FeatureSpacing) ++depth;
                return depth;
            }
        }
        public float LongestCellEdge(int depth)
        {
            ValidateDepth(depth);
            Vector3 size = Bounds.size;
            return Mathf.Max(size.x, Mathf.Max(size.y, size.z)) / (1 << depth);
        }
        internal int BudgetFor(int depth)
        {
            if (CellBudget < 0) throw new ArgumentOutOfRangeException(nameof(CellBudget));
            return CellBudget == 0 && (depth > 8 || FeatureSpacing > 0 || AdaptivePlanes) ? DefaultCellBudget : CellBudget;
        }
        internal static void ValidateDepth(int depth)
        {
            if (depth < 1 || depth > MaxDepth) throw new ArgumentOutOfRangeException(nameof(depth), $"Depth must be between 1 and {MaxDepth}.");
        }
        internal void Validate()
        {
            ValidateDepth(Depth);
            BudgetFor(SamplingDepth);
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
