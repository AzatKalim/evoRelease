using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    public class Temperature_status : Command
    {
        public Temperature_status()
        {
            is_answer = false;
            have_answer = true;
            can_send = true;
        }
        public override string ToString()
        {
            string buffer = "AXE.UPLD.STA ";
            return buffer;
        }
    }
}
