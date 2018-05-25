using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo_20_commands
{
    /// <summary>
    /// Включить или выключить температурную камеру
    /// </summary>
    public class PowerOnTemperatureCamera : Command
    {
        bool turn_on;
        public PowerOnTemperatureCamera(bool turn_on)
        {
            is_answer = false;
            have_answer = true;
            can_send = true;
            this.turn_on = turn_on;
        }
        public override string ToString()
        {
            string buffer = "CLIM.ETAT " + turn_on;
            return buffer;
        }
    }
}
