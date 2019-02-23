using Evo20.Commands.Abstract;

namespace Evo20.Commands.CommndsWithAnswer
{
    public class AxisRateQuery : CommandWithAnswer
    {
        readonly Axis _axis;
        public AxisRateQuery(Axis axis)
        {
            _axis = axis;
        }
        public override string ToString()
        {
            string buffer = "AXE.TELL.VIT "+ AxisToInt(_axis);
            return buffer;
        }
    }
}
