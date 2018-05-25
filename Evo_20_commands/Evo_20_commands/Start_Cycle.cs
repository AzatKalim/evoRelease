using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo_20_commands
{
    public class Start_Cycle:Command
    {
        string cycle_name;
        public Start_Cycle(string cycle_name)
        {
            is_answer = false;
            have_answer = true;
            can_send = true;
            this.cycle_name = cycle_name;
        }
        public override string ToString()
        {
            string buffer = "CLIM.CYCL" + cycle_name;
            return buffer;
        }
    }
}
