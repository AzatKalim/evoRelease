using Evo20.Commands.Abstract;

namespace Evo20.Commands.CommndsWithAnswer
{
    public class AxisPositionQuery : CommandWithAnswer
    {
        readonly Axis _axis;
        public  AxisPositionQuery(Axis axis)
        {
            _axis = axis;
        }

        public override string ToString() => $"AXE.TELL.POS {AxisToInt(_axis)}";
    }
}
