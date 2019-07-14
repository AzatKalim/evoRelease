using Evo20.Commands.Abstract;

namespace Evo20.Commands.CommndsWithAnswer
{
    public class RotaryJointTemperatureQuery : CommandWithAnswer
    {
        readonly Axis _axis;
        public RotaryJointTemperatureQuery(Axis axis)
        {
            _axis = axis;
        }
        public override string ToString()
        {
            return $"AXE.TELL.TJT {AxisToInt(_axis)}";
        }
    }
}
