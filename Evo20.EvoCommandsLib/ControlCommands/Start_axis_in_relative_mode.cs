using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Start_axis_in_relative_mode : ControlCommand
    {
        Axis axis;

        public Start_axis_in_relative_mode(Axis axis)
        {
            this.axis = axis;
        }
        public override string ToString()
        {
            return "AXE.STREEL " + AxisToInt(axis);
        }
    }
}
