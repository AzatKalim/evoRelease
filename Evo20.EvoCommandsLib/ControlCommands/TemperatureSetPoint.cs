using Evo20.Commands.Abstract;

namespace Evo20.Commands.ControlCommands
{
    public class TemperatureSetPoint : ControlCommand
    {
        readonly double _temperature;
        public TemperatureSetPoint(double temperature)
        {
            _temperature = temperature;
        }
        public override string ToString()
        {
            return $"CLIM.CONS {_temperature}";
        }
    }
}
