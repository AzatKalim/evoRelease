using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    public class Actual_temperature_query:Command
    {
        public static string Command = "CLIM.TEMP";

        public Actual_temperature_query()
        {
            is_answer = false;
            have_answer = true;
            can_send = true;
        }
        public override string ToString()
        {
            string buffer = "CLIM.TEMP";
            return buffer;
        }
    }
}
