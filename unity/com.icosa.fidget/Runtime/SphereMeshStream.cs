using System;
using UnityEngine;

namespace Icosa.Fidget
{
    /// <summary>Convenience sphere adapter. New geometry uses ShapeBuilder and MeshStream.</summary>
    public sealed class SphereMeshStream : IDisposable
    {
        private readonly CompiledShape _shape;
        private readonly Parameter _radius;
        private readonly ParameterValues _values;
        private MeshStream _stream;
        public Mesh Mesh { get; private set; }
        public bool IsPending => _stream != null && _stream.IsPending;
        public int VertexCount => _stream?.VertexCount ?? 0;
        public int TriangleCount => _stream?.TriangleCount ?? 0;
        private bool _disposed;
        public SphereMeshStream()
        {
            var builder = new ShapeBuilder();
            _radius = builder.Parameter("radius", 0.6f);
            _shape = builder.Compile(builder.Sphere(_radius));
            _values = _shape.CreateParameters();
            try { _stream = new MeshStream(_shape, new MeshSettings(new Bounds(Vector3.zero, Vector3.one * 2))); }
            catch { _shape.Dispose(); throw; }
            Mesh = _stream.Mesh;
        }
        public bool Schedule(float radius, int depth)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(SphereMeshStream));
            if (IsPending) return false;
            if (radius <= 0 || radius >= 1) throw new ArgumentOutOfRangeException(nameof(radius));
            _values.Set(_radius, radius);
            return _stream.TrySchedule(_values, depth);
        }
        public bool TryApply() => _stream.TryApply();
        public void Dispose() { if (_disposed) return; _disposed = true; _stream?.Dispose(); _shape.Dispose(); }
    }
}
