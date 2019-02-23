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
            string buffer = "CLIM.ETAT ";
            if (_turnOn)
                buffer += "1";
            else
                buffer += "0";
            return buffer;
        }
    }
}
