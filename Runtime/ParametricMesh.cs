using UnityEngine;

namespace Icosa.Fidget
{
    /// <summary>A shape component: define its expression once and update ordinary typed properties.</summary>
    public abstract class FidgetMesh<T> : FidgetMesh where T : FidgetMesh<T>
    {
        [Tooltip("Local-space region to mesh. Configure before enabling the component, or call Rebuild.")]
        public Bounds Bounds = new Bounds(Vector3.zero, Vector3.one * 2);
        public MeshNormals Normals = MeshNormals.Surface;
        private bool _defined;

        /// <summary>Called when first enabled, or explicitly rebuilt. Register changing inputs with shape.Parameter.</summary>
        protected abstract Expression Define(ShapeBuilder<T> shape);

        /// <summary>Rebuild after changing expression structure, bounds, or normal mode. Property changes need no rebuild.</summary>
        public void Rebuild()
        {
            if (!(this is T model)) throw new System.InvalidOperationException($"The type argument must match the component: {GetType().Name}.");
            var settings = new MeshSettings(Bounds, Detail, Normals);
            settings.Validate();
            var builder = new ShapeBuilder<T>();
            var shape = builder.Compile(Define(builder));
            try { Initialize(shape, model, settings); }
            catch { shape.Dispose(); throw; }
            _defined = true;
        }

        protected virtual void Reset() => Continuous = false;

        protected override void OnEnable()
        {
            if (!_defined) Rebuild();
            base.OnEnable();
        }
    }
}
