using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Phase : ControlCommand
    {
        Axis axis;
        // в градусах
        double phase;
        public Phase(Axis axis, double phase)
        {
            this.axis = axis;
            this.phase = phase;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.PHA " + phase;
            return buffer + ' ' + AxisToInt(axis);
        }
    }
}
