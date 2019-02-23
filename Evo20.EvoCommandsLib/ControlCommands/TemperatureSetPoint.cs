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
            string buffer = "CLIM.CONS " + _temperature;
            return buffer;
        }
    }
}
