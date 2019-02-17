using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Axis_Rate_Query : CommandWithAnswer
    {
        Axis axis;
        public Axis_Rate_Query(Axis axis)
        {
            this.axis = axis;
        }
        public override string ToString()
        {
            string buffer = "AXE.TELL.VIT "+ AxisToInt(axis);
            return buffer;
        }
    }
}
