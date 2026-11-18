using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>One shared definition, independently animated components.</summary>
    public class SharedShapeInstances : MonoBehaviour
    {
        public class SharedSphere : FidgetMesh<SharedSphere>
        {
            public float Radius { get; set; } = 0.55f;

            protected override Expression Define(ShapeBuilder<SharedSphere> shape) =>
                shape.Subtract(shape.Sphere(shape.Parameter(sphere => sphere.Radius)),
                               shape.Box(new Vector3(0.22f, 1, 0.22f)));
        }

        public Material Material;
        [Range(1, 16)] public int Count = 4;
        [Range(1, 6)] public int Detail = 6;
        private SharedSphere[] _meshes;

        private void OnEnable()
        {
            _meshes = new SharedSphere[Mathf.Clamp(Count, 1, 16)];
            try
            {
                for (int i = 0; i < _meshes.Length; ++i)
                {
                    var mesh = _meshes[i] = i == 0
                        ? new GameObject("Shared shape").AddComponent<SharedSphere>()
                        : _meshes[0].CreateSharedInstance(transform);
                    mesh.transform.SetParent(transform, false);
                    mesh.transform.localPosition = new Vector3((i - (_meshes.Length - 1) * 0.5f) * 1.6f, 0, 0);
                    mesh.Detail = Detail;
                    mesh.GetComponent<MeshRenderer>().sharedMaterial = Material;
                }
            }
            catch { OnDisable(); throw; }
        }

        private void Update()
        {
            for (int i = 0; i < _meshes.Length; ++i)
                _meshes[i].Radius = 0.55f + Mathf.Sin(Time.time + i) * 0.15f;
        }

        private void OnDisable()
        {
            if (_meshes != null) foreach (var mesh in _meshes) if (mesh != null) Destroy(mesh.gameObject);
            _meshes = null;
        }
    }
}
