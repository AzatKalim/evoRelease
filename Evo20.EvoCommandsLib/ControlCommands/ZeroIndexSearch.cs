using Evo20.Commands.Abstract;

namespace Evo20.Commands.ControlCommands
{
    public class ZeroIndexSearch : ControlCommand
    {
        private readonly Axis _axis;

        public ZeroIndexSearch(Axis axis)
        {
            _axis = axis;
        }
        public override string ToString()
        {
            return $"AXE.ZERO {AxisToInt(_axis)}";
        }
    }
}
