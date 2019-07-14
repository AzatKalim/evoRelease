using Evo20.Commands.Abstract;

namespace Evo20.Commands.ControlCommands
{
    public class AxisPower : ControlCommand
    {
        readonly Axis _axis;
        readonly bool _control;
        public AxisPower(Axis axis, bool control)
        {
            _axis = axis;
            _control = control;
        }
        public override string ToString()
        {
             return $"AXE.PWR {(_control ? 1 : 0)} {AxisToInt(_axis)}";
        }
    }
}
