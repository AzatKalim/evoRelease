using Evo20.Commands.Abstract;

namespace Evo20.Commands.ControlCommands
{
    public class AxisPosition: ControlCommand
    {
        readonly Axis _axis;
        readonly double _degree;
        public AxisPosition(Axis axis, double degree)
        {
            _axis = axis;
            _degree = degree;
        }

        public override string ToString()
        {
            return $"AXE.CONS.POS {_degree} {AxisToInt(_axis)}";
        }
    }
}
