using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    public class Ventilation_command:Command
    {
        int ventilation_mode;
        public Ventilation_command(int ventilation_mode)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
            this.ventilation_mode = ventilation_mode;
        }
        public override string ToString()
        {
            string buffer = "CLIM.VENT" + ventilation_mode;
            return buffer;
        }
    }
}
