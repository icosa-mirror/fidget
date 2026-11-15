using System;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;

namespace Icosa.Fidget
{
    /// <summary>Immutable native expression shared by independent mesh streams. Main-thread API.</summary>
    public sealed class CompiledShape : IDisposable
    {
        private sealed class ShapeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            internal ShapeHandle(IntPtr value) : base(true) { SetHandle(value); }
            protected override bool ReleaseHandle() { Native.fidget_shape_free(handle); return true; }
        }
        private readonly ShapeHandle _handle;
        internal readonly ShapeBuilder Builder;
        internal readonly float[] Defaults;
        private readonly Dictionary<string, Parameter> _parameters;
        internal CompiledShape(IntPtr handle, ShapeBuilder builder, float[] defaults, Dictionary<string, Parameter> parameters)
        { _handle = new ShapeHandle(handle); Builder = builder; Defaults = defaults; _parameters = parameters; }
        internal IntPtr CreateMesher()
        {
            bool retained = false;
            try
            {
                _handle.DangerousAddRef(ref retained);
                Native.Check(Native.fidget_mesher_create(_handle.DangerousGetHandle(), out var mesher), "create mesher");
                return mesher;
            }
            finally { if (retained) _handle.DangerousRelease(); }
        }
        public Parameter GetParameter(string name) => _parameters[name];
        public ParameterValues CreateParameters() => new ParameterValues(this);
        /// <summary>Existing mesh streams retain the native expression and remain usable.</summary>
        public void Dispose() => _handle.Dispose();
    }

    /// <summary>Reusable main-thread values. TrySchedule snapshots these into stream-owned native memory.</summary>
    public sealed class ParameterValues
    {
        internal readonly CompiledShape Shape;
        internal readonly float[] Values;
        internal ParameterValues(CompiledShape shape) { Shape = shape; Values = (float[])shape.Defaults.Clone(); }
        public void Set(Parameter parameter, float value)
        {
            if (parameter.Owner != Shape.Builder || parameter.Index >= Values.Length) throw new ArgumentException("Parameter does not belong to this compiled shape.", nameof(parameter));
            ShapeBuilder.Finite(value, nameof(value));
            Values[parameter.Index] = value;
        }
        public void Set(string name, float value) => Set(Shape.GetParameter(name), value);
    }
}
