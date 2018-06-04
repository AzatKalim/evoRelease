using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    public class Emergency_stop:Command
    {
        public Emergency_stop()
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
        }
        public override string ToString()
        {
            string buffer = "AXE.AU ";
            return buffer;
        }
    }
}
