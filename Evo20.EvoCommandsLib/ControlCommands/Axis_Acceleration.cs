using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Axis_Acceleration : ControlCommand
    {
        Axis axis;
        double speed_of_acceleration;
        public Axis_Acceleration(Axis axis, double speed_of_acceleration)
        {
            this.axis = axis;
            this.speed_of_acceleration = speed_of_acceleration;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.ACC " +speed_of_acceleration;
            return buffer + ' ' + AxisToInt(axis);
        }
    }
}
