using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.Commands
{
    public class Requested_axis_position_reached:Command
    {
        public static string Command = "AXE.TELL.POA";

        public Requested_axis_position_reached()
        {
            is_answer = false;
            have_answer = true;
            can_send = true;
        }
        public override string ToString()
        {
            string buffer = "AXE.TELL.POA";
            return buffer;
        }
    }
}
