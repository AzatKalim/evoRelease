using Evo20.Commands.Abstract;

namespace Evo20.Commands.ControlCommands
{
    class AxisDeceleration : ControlCommand
    {
        readonly Axis _axis;
        readonly double _speedOfBraking;
        public AxisDeceleration(Axis value, double speedOfBraking)
        {
            _axis = value;
            _speedOfBraking = speedOfBraking;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.DEC " + _speedOfBraking;
            return buffer + ' ' + _axis;
        }
    }
}
