using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Axis_Position: ControlCommand
    {
        Axis axis;
        double degree;
        public Axis_Position(Axis axis, double degree)
        {
            this.axis = axis;
            this.degree = degree;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.POS "+degree;         
            return buffer + ' ' + AxisToInt(axis);
        }
    }
}
