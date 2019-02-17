using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Axis_Position_Query : CommandWithAnswer
    {
        Axis axis;
        public  Axis_Position_Query(Axis axis)
        {
            this.axis = axis;
        }

        public override string ToString()
        {
            string buffer = "AXE.TELL.POS "+ AxisToInt(axis);
            return buffer;
        }
    }
}
