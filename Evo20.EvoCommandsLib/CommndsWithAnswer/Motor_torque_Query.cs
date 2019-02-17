using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Motor_torque_Query : CommandWithAnswer
    {
        Axis axis;
        public Motor_torque_Query(Axis axis, double phase)
        {
            this.axis = axis;
        }
        public override string ToString()
        {
            string buffer = "AXE.TELL.CPL "+AxisToInt(axis);
            return buffer;
        }
    }
}
