using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.Commands
{
    public class Actual_humidity_query:Command
    {
        public Actual_humidity_query()
        {
            is_answer = false;
            have_answer = true;
            can_send = true;
        }
        public override string ToString()
        {
            string buffer = "CLIM.HUM";
            return buffer;
        }
    }
}
