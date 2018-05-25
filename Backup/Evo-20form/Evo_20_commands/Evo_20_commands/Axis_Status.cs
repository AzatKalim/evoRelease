using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Evo_20_commands
{
    public class Axis_Status:Command
    {
        public Axis_Status()
        {
            is_answer = false;
            have_answer = true;
            can_send = true;
        }
       
        public override string ToString()
        {
            string buffer = "AXE.TELL.STA ";
            return buffer;
        }
    }
}
