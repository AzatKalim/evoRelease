using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    class Axis_Deceleration : ControlCommand
    {
        Axis axis;
        double speed_of_braking;
        public Axis_Deceleration(Axis value, double speed_of_braking)
        {
            axis = value;
            this.speed_of_braking = speed_of_braking;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.DEC " + speed_of_braking;
            return buffer + ' ' + axis;
        }
    }
}
