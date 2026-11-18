using UnityEngine;

namespace FidgetExamples
{
    // Along +X the closest point is the axial tip. Beyond it the plane bound
    // grows by 1/sqrt(3) for every unit of actual separation.
    public class OctahedronProbe : MonoBehaviour
    {
        public OctahedronBoundExample Target;
        [Range(0.02f, 0.45f)] public float TipDistance = 0.25f;
        public Transform Marker;
        public TextMesh Readout;

        public void Refresh()
        {
            Marker.localPosition = Vector3.right * (Target.Radius + TipDistance);
            Readout.text = $"Exact {TipDistance:F2}   Bound {TipDistance / Mathf.Sqrt(3):F2}";
        }

        void Update() => Refresh();
    }
}
