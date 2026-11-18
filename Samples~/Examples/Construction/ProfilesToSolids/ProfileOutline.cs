using UnityEngine;

namespace FidgetExamples
{
    // Displays each source profile beside the solid in its own XY plane.
    public class ProfileOutline : MonoBehaviour
    {
        public LineRenderer Line;
        public LoftExample Loft;
        public Vector2 HalfSize;
        public float CornerRadius;
        public bool Circle;
        private Vector2 previous;

        public void Refresh()
        {
            var size = Loft == null ? HalfSize : new Vector2(Loft.EndWidth, HalfSize.y);
            if (size == previous) return;
            previous = size;
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
                var point = Circle ? direction * size.x : center * (size - Vector2.one * CornerRadius) + direction * CornerRadius;
                Line.SetPosition(i, point);
            }
        }

        void Update() => Refresh();
    }
}
