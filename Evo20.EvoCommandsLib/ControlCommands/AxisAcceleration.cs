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
            return $"AXE.CONS.ACC {_speedOfAcceleration} {AxisToInt(_axis)}";
        }
    }
}
