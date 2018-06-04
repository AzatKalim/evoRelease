using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    public class Requested_axis_position_reached:Command
    {
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
