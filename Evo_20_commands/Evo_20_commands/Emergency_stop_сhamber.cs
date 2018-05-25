using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo_20_commands
{
    /// <summary>
    /// Екстренное отключение тепловой камеры
    /// </summary>
    public class Emergency_stop_сhamber:Command
    {
        public Emergency_stop_сhamber(bool light_mode)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
        }
        public override string ToString()
        {
            string buffer = "CLIM.AU";
            return buffer;
        }
    }
}
