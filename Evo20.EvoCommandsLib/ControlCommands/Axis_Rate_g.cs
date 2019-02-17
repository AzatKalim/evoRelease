using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Axis_Rate_g : ControlCommand
    {
        Axis axis;
        double speed_of_rate_in_g;
        public Axis_Rate_g(Axis axis, double speed_of_rate)
        {
            this.axis = axis;
            this.speed_of_rate_in_g = speed_of_rate;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.VITG " + speed_of_rate_in_g;
            return buffer + ' ' + AxisToInt(axis);
        }
    }
}
