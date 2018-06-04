using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    public class Temperature_Set_point:Command
    {
        double temperature;
        public Temperature_Set_point(double temperature)
        {
            is_answer = false;
            have_answer = true;
            can_send = true;
            this.temperature = temperature;
        }
        public override string ToString()
        {
            string buffer = "CLIM.CONS " + temperature;
            return buffer;
        }
    }
}
