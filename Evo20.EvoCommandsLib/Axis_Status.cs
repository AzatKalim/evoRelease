using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Evo20.EvoCommandsLib
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
