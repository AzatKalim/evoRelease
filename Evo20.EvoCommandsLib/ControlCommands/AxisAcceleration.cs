using Evo20.Commands.Abstract;

namespace Evo20.Commands.ControlCommands
{
    public class AxisAcceleration : ControlCommand
    {
        readonly Axis _axis;
        readonly double _speedOfAcceleration;
        public AxisAcceleration(Axis axis, double speedOfAcceleration)
        {
            _axis = axis;
            _speedOfAcceleration = speedOfAcceleration;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.ACC " +_speedOfAcceleration;
            return buffer + ' ' + AxisToInt(_axis);
        }
    }
}
