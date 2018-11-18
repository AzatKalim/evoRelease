using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.Commands
{
    public class Light_command:Command
    {
        bool light_mode;
        public Light_command(bool light_mode)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
            this.light_mode = light_mode;
        }
        public override string ToString()
        {
            string buffer = "CLIM.ECL" + light_mode;
            return buffer;
        }
    }
}
