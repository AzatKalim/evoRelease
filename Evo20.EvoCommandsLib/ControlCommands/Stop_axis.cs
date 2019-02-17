using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Stop_axis : ControlCommand
    {
        Axis axis;

        public Stop_axis(Axis axis)
        {
            this.axis = axis;
        }
        public override string ToString()
        {
            return "AXE.STOP " + AxisToInt(axis);
        }
    }
}
