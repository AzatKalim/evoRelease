using Evo20.Commands.Abstract;

namespace Evo20.Commands.ControlCommands
{
    public class TemperatureSlopeSetPoint : ControlCommand
    {
        readonly double _slope;
        public TemperatureSlopeSetPoint(double slope)
        {
            _slope = slope;
        }
        public override string ToString()
        {
            return $"CLIM.RAMP { _slope}";
        }
    }
}
