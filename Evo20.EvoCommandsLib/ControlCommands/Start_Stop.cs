using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    /// <summary>
    /// Включить или выключить температурную камеру
    /// </summary>
    public class PowerOnTemperatureCamera : ControlCommand
    {
        bool turn_on;
        public PowerOnTemperatureCamera(bool turn_on)
        {
            this.turn_on = turn_on;
        }
        public override string ToString()
        {
            string buffer = "CLIM.ETAT ";
            if (turn_on)
                buffer += "1";
            else
                buffer += "0";
            return buffer;
        }
    }
}
