using Icosa.Fidget;

namespace FidgetExamples
{
    /// <summary>The unpatterned rotor, with one off-centre ventilation slot.</summary>
    public class SingleSlotExample : RotorExample<SingleSlotExample>
    {
        protected override Expression Define(ShapeBuilder<SingleSlotExample> s) =>
            s.Subtract(Disc(s), Slot(s));

    }
}
