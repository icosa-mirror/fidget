using System;
using System.Collections.Generic;

namespace Icosa.Fidget
{
    /// <summary>Defines an expression using ordinary typed model properties. Bindings are created once.</summary>
    public sealed class ShapeBuilder<T> : ShapeBuilder where T : class
    {
        private readonly List<Func<T, float>> _readers = new List<Func<T, float>>();
        private readonly List<Parameter> _slots = new List<Parameter>();

        /// <summary>The accessor is ordinary compiled C#: no reflection, expression compilation, or name lookup.</summary>
        public Expression Parameter(Func<T, float> read)
        {
            if (read == null) throw new ArgumentNullException(nameof(read));
            var slot = base.Parameter();
            _slots.Add(slot);
            _readers.Add(read);
            return slot;
        }

        public new CompiledShape<T> Compile(Expression root) =>
            new CompiledShape<T>(base.Compile(root), _slots.ToArray(), _readers.ToArray());
    }

    /// <summary>A compiled expression with typed property bindings, reusable across model instances.</summary>
    public sealed class CompiledShape<T> : IDisposable where T : class
    {
        public CompiledShape Shape { get; }
        private readonly Parameter[] _slots;
        private readonly Func<T, float>[] _readers;

        internal CompiledShape(CompiledShape shape, Parameter[] slots, Func<T, float>[] readers)
        { Shape = shape; _slots = slots; _readers = readers; }

        /// <summary>Bind independent values to a stream. Keep the model on the main thread.</summary>
        public ParameterValues CreateParameters(T model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            var values = Shape.CreateParameters();
            values.ReadBindings = () =>
            {
                for (int i = 0; i < _slots.Length; ++i)
                {
                    float value = _readers[i](model);
                    ShapeBuilder.Finite(value, nameof(model));
                    values.Values[_slots[i].Index] = value;
                }
            };
            values.CaptureBindings();
            return values;
        }

        internal CompiledShape<T> Retain() => new CompiledShape<T>(Shape.Retain(), _slots, _readers);
        public void Dispose() => Shape.Dispose();
    }
}
