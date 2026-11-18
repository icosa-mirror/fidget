using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    // Displays each source profile beside the solid in its own XY plane.
    public class ProfileOutline : MonoBehaviour
    {
        public LineRenderer Line;
        public FidgetMesh Source;
        public bool Circle;
        private Vector2 previous;
        private float previousRadius;

        public void Refresh()
        {
            Vector2 size;
            float radius = 0;
            if (Source is ExtrusionExample extrusion)
            {
                size = new Vector2(extrusion.HalfWidth, extrusion.HalfHeight);
                radius = extrusion.CornerRadius;
            }
            else if (Source is LoftExample loft)
            {
                size = Circle ? Vector2.one * loft.StartRadius : new Vector2(loft.EndWidth, loft.EndHeight);
                radius = Circle ? 0 : 0.06f;
            }
            else if (Source is RevolutionExample revolution) size = Vector2.one * revolution.ProfileRadius;
            else return;
            if (size == previous && radius == previousRadius) return;
            previous = size;
            previousRadius = radius;
            const int samplesPerCorner = 12;
            int count = 4 * samplesPerCorner;
            Line.positionCount = count;
            for (int i = 0; i < count; i++)
            {
                int corner = i / samplesPerCorner;
                float t = (i % samplesPerCorner) / (samplesPerCorner - 1f);
                float angle = (corner + t) * Mathf.PI * 0.5f;
                var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                var center = new Vector2(corner == 0 || corner == 3 ? 1 : -1, corner < 2 ? 1 : -1);
                var point = Circle ? direction * size.x : center * (size - Vector2.one * radius) + direction * radius;
                Line.SetPosition(i, point);
            }
        }

        void Update() => Refresh();
    }
}
