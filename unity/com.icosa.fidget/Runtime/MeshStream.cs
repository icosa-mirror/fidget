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
    public sealed class MeshStream : IDisposable
    {
        private struct Result
        {
            public int Status;
            public Native.SamplingStats Work;
            public Native.Counts Counts;
            public Native.Bounds Bounds;
        }

        // No BurstCompile: the native Rust implementation performs the computation.
        private unsafe struct GenerateJob : IJob
        {
            // Exclusively owned by this stream; Dispose completes the job before freeing it.
            [NativeDisableUnsafePtrRestriction] public IntPtr Mesher;
            public MeshSettings Settings;
            [ReadOnly] public NativeArray<float> Parameters;
            public Mesh.MeshDataArray Output;
            [ReadOnly] public NativeArray<VertexAttributeDescriptor> Layout;
            public NativeArray<Result> Results;

            public void Execute()
            {
                var result = new Result { Status = -1 };
                var handle = IntPtr.Zero;
                try
                {
                    var bounds = new Native.Bounds { Min = Settings.Bounds.min, Max = Settings.Bounds.max };
                    result.Status = Native.fidget_mesher_generate_with_budget(Mesher, Parameters.GetUnsafeReadOnlyPtr(),
                        (uint)Parameters.Length, in bounds, (uint)Settings.Depth, (uint)Settings.CellBudget, out result.Work, out handle);
                    if (result.Status != 0) return;
                    if (Settings.Normals == MeshNormals.Surface)
                    {
                        result.Status = Native.fidget_mesh_prepare_surface_normals(Mesher, handle);
                        if (result.Status != 0) return;
                    }
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
                    if (Settings.Normals != MeshNormals.None)
                    {
                        var normals = data.GetVertexData<Vector3>(1);
                        int normalStatus = Native.fidget_mesh_write_normals(handle, normals.GetUnsafePtr(), result.Counts.Vertices);
                        if (normalStatus != 0) { result.Status = normalStatus; return; }
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

        private readonly CompiledShape _shape;
        private IntPtr _mesher;
        private NativeArray<float> _parameters;
        private NativeArray<VertexAttributeDescriptor> _layout;
        private NativeArray<Result> _results;
        private Mesh.MeshDataArray _output;
        private JobHandle _job;
        private bool _pending;
        private bool _disposed;

        public Mesh Mesh { get; private set; }
        public MeshSettings Settings { get; }
        public bool IsPending => _pending;
        public int VertexCount { get; private set; }
        public int TriangleCount { get; private set; }
        public bool CellBudgetExceeded { get; private set; }
        public uint LastVisitedCells { get; private set; }
        public int AppliedDepth { get; private set; }
        public float AppliedCellSpacing => AppliedDepth == 0 ? 0 : Settings.LongestCellEdge(AppliedDepth);
        private int _pendingDepth;

        // Only read the snapshot after its job completed.
        internal bool HasScheduledValues(ParameterValues values)
        {
            for (int i = 0; i < _parameters.Length; ++i)
                if (_parameters[i] != values.Values[i]) return false;
            return true;
        }

        public MeshStream(CompiledShape shape, MeshSettings settings)
        {
            if (shape == null) throw new ArgumentNullException(nameof(shape));
            settings.Validate();
            Native.CheckAbi();
            _shape = shape;
            Settings = settings;
            try
            {
                _mesher = shape.CreateMesher();
                _parameters = new NativeArray<float>(shape.Defaults.Length, Allocator.Persistent);
                _layout = new NativeArray<VertexAttributeDescriptor>(settings.Normals != MeshNormals.None ? 2 : 1, Allocator.Persistent);
                _layout[0] = new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0);
                if (_layout.Length == 2) _layout[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, 1);
                _results = new NativeArray<Result>(1, Allocator.Persistent);
                Mesh = new Mesh { name = "Fidget streamed mesh" };
                Mesh.MarkDynamic();
            }
            catch { Dispose(); throw; }
        }

        /// <summary>Returns false while busy. Call again with the latest parameters; no queue is kept.</summary>
        public bool TrySchedule(ParameterValues parameters) => TrySchedule(parameters, Settings.SamplingDepth);

        /// <summary>Overrides depth for this request without rebuilding the shape or replacing the Mesh.</summary>
        public bool TrySchedule(ParameterValues parameters, int depth)
            => TrySchedule(parameters, depth, Settings.BudgetFor(depth), true);

        internal bool TrySchedule(ParameterValues parameters, int depth, int cellBudget, bool captureBindings)
        {
            ThrowIfDisposed();
            if (_pending) return false;
            MeshSettings.ValidateDepth(depth);
            var settings = Settings;
            settings.Depth = depth;
            settings.CellBudget = cellBudget;
            if (parameters == null || parameters.Shape != _shape) throw new ArgumentException("Values must come from the stream's compiled shape.", nameof(parameters));
            if (captureBindings) parameters.CaptureBindings();
            // A dedicated snapshot prevents writes on the main thread from racing the worker.
            _parameters.CopyFrom(parameters.Values);
            _output = Mesh.AllocateWritableMeshData(1);
            _results[0] = new Result { Status = -1 };
            try
            {
                _job = new GenerateJob
                {
                    Mesher = _mesher, Settings = settings, Parameters = _parameters,
                    Output = _output, Layout = _layout, Results = _results
                }.Schedule();
                _pending = true;
                _pendingDepth = depth;
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
            LastVisitedCells = result.Work.Visited;
            CellBudgetExceeded = result.Status == Native.CellBudgetExceeded;
            if (result.Status != 0)
            {
                _output.Dispose();
                if (CellBudgetExceeded) return false;
                throw new InvalidOperationException($"Fidget mesh update failed with status {result.Status}.");
            }
            // Rust owns the topology and guarantees that its indices address its vertices.
            // Avoid a second index/bounds scan on the main thread.
            Mesh.ApplyAndDisposeWritableMeshData(_output, Mesh,
                MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);
            Mesh.bounds = ToUnityBounds(result.Bounds);
            VertexCount = (int)result.Counts.Vertices;
            TriangleCount = (int)result.Counts.Indices / 3;
            AppliedDepth = _pendingDepth;
            return true;
        }

        private static Bounds ToUnityBounds(Native.Bounds bounds)
        {
            return new Bounds((bounds.Min + bounds.Max) * 0.5f, bounds.Max - bounds.Min);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(MeshStream));
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
            if (_results.IsCreated) _results.Dispose();
            if (_layout.IsCreated) _layout.Dispose();
            if (_parameters.IsCreated) _parameters.Dispose();
            Native.fidget_mesher_free(_mesher);
            _mesher = IntPtr.Zero;
            if (Application.isPlaying) UnityEngine.Object.Destroy(Mesh);
            else UnityEngine.Object.DestroyImmediate(Mesh);
            _disposed = true;
        }
    }
}
