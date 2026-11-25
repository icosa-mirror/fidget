using UnityEngine;

namespace Icosa.Fidget
{
    /// <summary>A shape component: define its expression once and update ordinary typed properties.</summary>
    public abstract class FidgetMesh<T> : FidgetMesh where T : FidgetMesh<T>
    {
        [Tooltip("Local-space region to mesh. Configure before enabling the component, or call Rebuild.")]
        public Bounds Bounds = new Bounds(Vector3.zero, Vector3.one * 2);
        public MeshNormals Normals = MeshNormals.Surface;
        private CompiledShape<T> _definition;

        /// <summary>Called when first enabled, or explicitly rebuilt. Register changing inputs with shape.Parameter.</summary>
        protected abstract Expression Define(ShapeBuilder<T> shape);

        /// <summary>Rebuild after changing expression structure, bounds, or normal mode. Property changes need no rebuild.</summary>
        public void Rebuild()
        {
            if (!(this is T model)) throw new System.InvalidOperationException($"The type argument must match the component: {GetType().Name}.");
            var settings = new MeshSettings(Bounds, Detail, Normals, FeatureSpacing, CellBudget, AdaptivePlanes) { SurfaceError = this.SurfaceError };
            settings.Validate();
            var builder = new ShapeBuilder<T>();
            var shape = builder.Compile(Define(builder));
            try { Initialize(shape, model, settings); }
            catch { shape.Dispose(); throw; }
            _definition = shape;
        }

        /// <summary>Use the source's current expression with this component's own properties and settings.</summary>
        public void ShareDefinition(T source)
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (ReferenceEquals(source, this)) throw new System.ArgumentException("A component cannot share its own definition.", nameof(source));
            if (source._definition == null) source.Rebuild();
            var shape = source._definition.Retain();
            try { Initialize(shape, (T)this, new MeshSettings(Bounds, Detail, Normals, FeatureSpacing, CellBudget, AdaptivePlanes) { SurfaceError = this.SurfaceError }); }
            catch { shape.Dispose(); throw; }
            _definition = shape;
        }

        /// <summary>Create a plain shape GameObject sharing this expression, settings, and materials.
        /// Shape properties start with their normal defaults. Other components and children are not copied.</summary>
        public T CreateSharedInstance(Transform parent = null)
        {
            var go = new GameObject(typeof(T).Name);
            go.SetActive(false); // Assign the shared definition before OnEnable can build one.
            try
            {
                go.transform.SetParent(parent, false);
                var instance = go.AddComponent<T>();
                instance.Bounds = Bounds;
                instance.Detail = Detail;
                instance.FeatureSpacing = FeatureSpacing;
                instance.CellBudget = CellBudget;
                instance.AdaptivePlanes = AdaptivePlanes;
                instance.SurfaceError = SurfaceError;
                instance.Normals = Normals;
                instance.Continuous = Continuous;
                instance.ShareDefinition((T)this);
                instance.GetComponent<MeshRenderer>().sharedMaterials = GetComponent<MeshRenderer>().sharedMaterials;
                go.SetActive(true);
                return instance;
            }
            catch
            {
                if (Application.isPlaying) Destroy(go);
                else DestroyImmediate(go);
                throw;
            }
        }

        protected virtual void Reset() => Continuous = false;

        protected override void OnEnable()
        {
            if (_definition == null) Rebuild();
            base.OnEnable();
        }
    }
}
