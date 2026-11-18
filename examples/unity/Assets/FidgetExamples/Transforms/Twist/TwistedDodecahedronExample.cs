using Icosa.Fidget;
using UnityEngine;
using UnityEngine.Serialization;
namespace FidgetExamples
{
    public class TwistedDodecahedronExample : FidgetMesh<TwistedDodecahedronExample>
    {
        [field: FormerlySerializedAs("<Bob>k__BackingField")]
        [field: SerializeField, Range(0f, 10f)] public float Twist { get; set; } = 1f;
        protected override Expression Define(ShapeBuilder<TwistedDodecahedronExample> shape)
        {
            return shape.TwistY(shape.DodecahedronBound(1), shape.Parameter(mesh => mesh.Twist));
        }
    }
}
