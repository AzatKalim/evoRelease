using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Axis_Rate : ControlCommand
    {
        Axis axis;
        double speed_of_rate;
        public Axis_Rate(Axis axis, double speed_of_rate)
        {
            this.axis = axis;
            this.speed_of_rate = speed_of_rate;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.VIT " + speed_of_rate;         
            return buffer + ' ' + AxisToInt(axis);
        }
    }
}
