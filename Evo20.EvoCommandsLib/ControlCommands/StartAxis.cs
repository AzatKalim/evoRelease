using Evo20.Commands.Abstract;

namespace Evo20.Commands.ControlCommands
{
    public class StartAxis : ControlCommand
    {
        readonly Axis _axis;

        public StartAxis(Axis axis)
        {
            _axis = axis;
        }
        public override string ToString()
        {
            return "AXE.STRT " + AxisToInt(_axis);
        }
    }
}
