using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Icosa.Fidget
{
    internal static unsafe class Native
    {
        internal const uint AbiVersion = 6;

        [StructLayout(LayoutKind.Sequential)]
        internal struct Node
        {
            public uint Opcode, A, B, C, D;
            public float Value;
        }

        internal static void Check(int status, string operation)
        {
            if (status != 0)
                throw new InvalidOperationException($"Fidget {operation} failed: {(status == 1 ? "invalid argument" : status == 2 ? "meshing failed" : status == 3 ? "native panic" : $"status {status}")}.");
        }

        internal static void CheckAbi()
        {
            uint version = fidget_native_abi_version();
            if (version != AbiVersion)
                throw new InvalidOperationException($"Fidget ABI {AbiVersion} required; found {version}. Install the matching UPM package including its native libraries.");
        }
#if UNITY_IOS && !UNITY_EDITOR
        private const string Library = "__Internal";
#else
        private const string Library = "fidget_native";
#endif

        [StructLayout(LayoutKind.Sequential)]
        internal struct Counts
        {
            public uint Vertices;
            public uint Indices;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Bounds
        {
            public Vector3 Min;
            public Vector3 Max;
        }

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint fidget_native_abi_version();
        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int fidget_mesh_sphere(float radius, uint depth, out IntPtr mesh);
        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int fidget_mesh_get_counts(IntPtr mesh, out Counts counts);
        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int fidget_mesh_copy_to(IntPtr mesh, void* vertices, uint vertexCapacity,
            void* indices, uint indexCapacity, out Bounds bounds);
        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void fidget_mesh_free(IntPtr mesh);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int fidget_shape_compile(Node* nodes, uint count, uint parameterCount, out IntPtr shape);
        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void fidget_shape_free(IntPtr shape);
        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int fidget_mesher_create(IntPtr shape, out IntPtr mesher);
        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int fidget_mesher_generate(IntPtr mesher, void* values, uint count, in Bounds bounds, uint depth, out IntPtr mesh);
        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void fidget_mesher_free(IntPtr mesher);
        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int fidget_mesh_write_normals(IntPtr mesh, void* normals, uint capacity);
        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int fidget_mesh_prepare_surface_normals(IntPtr mesher, IntPtr mesh);
    }
}
