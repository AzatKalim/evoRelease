using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Start_axis : ControlCommand
    {
        Axis axis;

        public Start_axis(Axis axis)
        {
            this.axis = axis;
        }
        public override string ToString()
        {
            return "AXE.STRT " + AxisToInt(axis);
        }
    }
}
