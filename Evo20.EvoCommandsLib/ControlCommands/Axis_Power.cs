using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Axis_Power : ControlCommand
    {
        Axis axis;
        bool control;
        public Axis_Power(Axis axis, bool control)
        {
            this.axis = axis;
            this.control = control;
        }
        public override string ToString()
        {
            string buffer = "AXE.PWR ";
            if (control)
                buffer += '1';
            else
                buffer += '0';
            return buffer + ' ' + AxisToInt(axis);
        }
    }
}
