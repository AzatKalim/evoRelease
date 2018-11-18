using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Evo20.Commands
{
    public class Axis_Status:Command
    {
        public static string Command = "AXE.TELL.STA ";

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
