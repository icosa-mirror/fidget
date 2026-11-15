using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Icosa.Fidget
{
    internal static unsafe class Native
    {
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
    }
}
