using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples
{
    /// <summary>Only arranges the recipes; shape definitions stay in their individual component scripts.</summary>
    internal static class GalleryLayout
    {
        internal static Expression Profiles(ShapeBuilder shape, params Expression[] profiles)
        {
            var solids = new Expression[profiles.Length];
            for (int i = 0; i < profiles.Length; i++) solids[i] = shape.ExtrudeZExact(profiles[i], -0.12f, 0.12f);
            return Tiles(shape, solids);
        }
        internal static Expression Tiles(ShapeBuilder shape, params Expression[] parts)
        {
            int columns = parts.Length > 4 ? 3 : 2;
            int rows = (parts.Length + columns - 1) / columns;
            var placed = new Expression[parts.Length];
            for (int i = 0; i < parts.Length; i++)
                placed[i] = shape.Translate(shape.ScaleUniformExact(parts[i], 0.7f),
                    new Vector3((i % columns - (columns - 1) * 0.5f) * 1.15f,
                                ((rows - 1) * 0.5f - i / columns) * 1.15f, 0));
            return shape.Union(placed);
        }
        internal static void JoinInputs(ShapeBuilder shape, out Expression a, out Expression b)
        {
            a = shape.BoxExact(new Expression3(0.6f, 0.22f, 0.3f));
            b = shape.Translate(shape.BoxExact(new Expression3(0.22f, 0.6f, 0.3f)), new Vector3(-0.25f, 0.25f, 0));
        }
    }
}
