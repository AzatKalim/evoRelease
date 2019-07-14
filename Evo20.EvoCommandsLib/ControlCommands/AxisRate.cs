using Evo20.Commands.Abstract;

namespace Evo20.Commands.ControlCommands
{
    public class AxisRate : ControlCommand
    {
        readonly Axis _axis;
        readonly double _speedOfRate;
        public AxisRate(Axis axis, double speedOfRate)
        {
            _axis = axis;
            _speedOfRate = speedOfRate;
        }
        public override string ToString()
        {
            return $"AXE.CONS.VIT {_speedOfRate} {AxisToInt(_axis)}";
        }
    }
}
