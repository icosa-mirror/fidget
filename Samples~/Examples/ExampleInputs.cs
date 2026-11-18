using Icosa.Fidget;
using UnityEngine;
namespace FidgetExamples
{
    // Common intersecting inputs for comparing join operators.
    internal static class ExampleInputs
    {
        internal static void JoinInputs(ShapeBuilder shape, out Expression a, out Expression b)
        {
            a = shape.BoxExact(new Expression3(0.6f, 0.22f, 0.3f));
            b = shape.Translate(shape.BoxExact(new Expression3(0.22f, 0.6f, 0.3f)), new Vector3(-0.25f, 0.25f, 0));
        }
    }
}
