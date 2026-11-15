using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

namespace Icosa.Fidget
{
    /// <summary>
    /// Main-thread controller. One non-Burst worker job generates and writes each update.
    /// The current mesh remains visible while the next update is in flight.
    /// </summary>
    public sealed class SphereMeshStream : IDisposable
    {
        private struct Result
        {
            public int Status;
            public Native.Counts Counts;
            public Native.Bounds Bounds;
        }

        // No BurstCompile: the native Rust implementation performs the computation.
        private unsafe struct GenerateJob : IJob
        {
            public float Radius;
            public uint Depth;
            public Mesh.MeshDataArray Output;
            [ReadOnly] public NativeArray<VertexAttributeDescriptor> Layout;
            public NativeArray<Result> Results;

            public void Execute()
            {
                var result = new Result { Status = -1 };
                var handle = IntPtr.Zero;
                try
                {
                    result.Status = Native.fidget_mesh_sphere(Radius, Depth, out handle);
                    if (result.Status != 0) return;
                    result.Status = Native.fidget_mesh_get_counts(handle, out result.Counts);
                    if (result.Status != 0) return;
                    if (result.Counts.Vertices > int.MaxValue || result.Counts.Indices > int.MaxValue)
                    {
                        result.Status = -2;
                        return;
                    }

                    // Unexpected managed/Unity exceptions must not leave a successful result.
                    result.Status = -1;
                    var data = Output[0];
                    data.SetVertexBufferParams((int)result.Counts.Vertices, Layout);
                    data.SetIndexBufferParams((int)result.Counts.Indices, IndexFormat.UInt32);
                    var positions = data.GetVertexData<Vector3>();
                    var indices = data.GetIndexData<uint>();
                    int copyStatus = Native.fidget_mesh_copy_to(handle,
                        positions.GetUnsafePtr(), result.Counts.Vertices,
                        indices.GetUnsafePtr(), result.Counts.Indices, out result.Bounds);
                    if (copyStatus != 0)
                    {
                        result.Status = copyStatus;
                        return;
                    }
                    data.subMeshCount = 1;
                    data.SetSubMesh(0, new SubMeshDescriptor(0, (int)result.Counts.Indices)
                    {
                        vertexCount = (int)result.Counts.Vertices,
                        bounds = ToUnityBounds(result.Bounds)
                    }, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);
                    result.Status = 0;
                }
                finally
                {
                    Native.fidget_mesh_free(handle);
                    Results[0] = result;
                }
            }
        }

        private NativeArray<VertexAttributeDescriptor> _layout;
        private NativeArray<Result> _results;
        private Mesh.MeshDataArray _output;
        private JobHandle _job;
        private bool _pending;
        private bool _disposed;

        public Mesh Mesh { get; }
        public bool IsPending => _pending;
        public int VertexCount { get; private set; }
        public int TriangleCount { get; private set; }

        public SphereMeshStream()
        {
            // Load/validate the plugin on the main thread, before entering a worker job.
            uint abi = Native.fidget_native_abi_version();
            if (abi != 2) throw new InvalidOperationException($"Fidget ABI 2 required; found {abi}.");
            _layout = new NativeArray<VertexAttributeDescriptor>(1, Allocator.Persistent);
            _layout[0] = new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3);
            _results = new NativeArray<Result>(1, Allocator.Persistent);
            Mesh = new Mesh { name = "Fidget streamed mesh" };
            Mesh.MarkDynamic();
        }

        /// <summary>Returns false while busy. Call again with the latest parameters; no queue is kept.</summary>
        public bool Schedule(float radius, int depth)
        {
            ThrowIfDisposed();
            if (_pending) return false;
            if (float.IsNaN(radius) || float.IsInfinity(radius) || radius <= 0 || radius >= 1)
                throw new ArgumentOutOfRangeException(nameof(radius));
            if (depth < 1 || depth > 8) throw new ArgumentOutOfRangeException(nameof(depth));
            _output = Mesh.AllocateWritableMeshData(1);
            _results[0] = new Result { Status = -1 };
            try
            {
                _job = new GenerateJob
                {
                    Radius = radius, Depth = (uint)depth,
                    Output = _output, Layout = _layout, Results = _results
                }.Schedule();
                _pending = true;
                JobHandle.ScheduleBatchedJobs();
                return true;
            }
            catch
            {
                _output.Dispose();
                throw;
            }
        }

        /// <summary>Applies a completed update. Never waits for an unfinished job.</summary>
        public bool TryApply()
        {
            ThrowIfDisposed();
            if (!_pending || !_job.IsCompleted) return false;
            _job.Complete();
            _pending = false;
            var result = _results[0];
            if (result.Status != 0)
            {
                _output.Dispose();
                throw new InvalidOperationException($"Fidget mesh update failed with status {result.Status}.");
            }
            // Rust owns the topology and guarantees that its indices address its vertices.
            // Avoid a second index/bounds scan on the main thread.
            Mesh.ApplyAndDisposeWritableMeshData(_output, Mesh,
                MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);
            Mesh.bounds = ToUnityBounds(result.Bounds);
            VertexCount = (int)result.Counts.Vertices;
            TriangleCount = (int)result.Counts.Indices / 3;
            return true;
        }

        private static Bounds ToUnityBounds(Native.Bounds bounds)
        {
            return new Bounds((bounds.Min + bounds.Max) * 0.5f, bounds.Max - bounds.Min);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(SphereMeshStream));
        }

        /// <summary>Must run on the main thread. Teardown waits for any in-flight native call.</summary>
        public void Dispose()
        {
            if (_disposed) return;
            if (_pending)
            {
                _job.Complete();
                _output.Dispose();
                _pending = false;
            }
            _results.Dispose();
            _layout.Dispose();
            if (Application.isPlaying) UnityEngine.Object.Destroy(Mesh);
            else UnityEngine.Object.DestroyImmediate(Mesh);
            _disposed = true;
        }
    }
}
