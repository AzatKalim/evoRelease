using Evo20.Commands.Abstract;

namespace Evo20.Commands.ControlCommands
{
    public class StopAxis : ControlCommand
    {
        readonly Axis _axis;

        public StopAxis(Axis axis)
        {
            _axis = axis;
        }
        public override string ToString()
        {
            return "AXE.STOP " + AxisToInt(_axis);
        }
    }
}
