using Icosa.Fidget;

namespace FidgetExamples
{
    /// <summary>The unpatterned rotor, with one off-centre ventilation slot.</summary>
    public class SingleSlotExample : FidgetMesh<SingleSlotExample>
    {
        protected override Expression Define(ShapeBuilder<SingleSlotExample> s) =>
            s.Subtract(Disc(s), Slot(s));

        // Both repeated examples start with this same XY rotor and through-cut.
        internal static Expression Disc(ShapeBuilder s)
        {
            var annulus = s.Subtract(s.Circle(0.74f), s.Circle(0.14f));
            return s.ExtrudeZExact(annulus, -0.055f, 0.055f);
        }

        internal static Expression Slot(ShapeBuilder s)
        {
            var segment = s.Segment(new Expression2(0.30f, -0.025f), new Expression2(0.58f, 0.09f));
            return s.Round(segment, 0.064f);
        }
    }
}
