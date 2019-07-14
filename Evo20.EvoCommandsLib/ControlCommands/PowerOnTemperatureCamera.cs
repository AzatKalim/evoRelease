using Evo20.Commands.Abstract;

namespace Evo20.Commands.ControlCommands
{   
    public class PowerOnTemperatureCamera : ControlCommand
    {
        readonly bool _turnOn;
        public PowerOnTemperatureCamera(bool turnOn)
        {
            _turnOn = turnOn;
        }
        public override string ToString()
        {
            return $"CLIM.ETAT {(_turnOn ? 1 : 0)}";
        }
    }
}
