using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Zero_Index_Search : ControlCommand
    {
        Axis axis;

        public Zero_Index_Search(Axis axis)
        {
            this.axis = axis;
        }
        public override string ToString()
        {
            return "AXE.ZERO " + AxisToInt(axis);
        }
    }
}
