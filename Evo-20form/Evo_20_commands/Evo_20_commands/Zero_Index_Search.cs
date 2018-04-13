using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo_20_commands
{
    public class Zero_Index_Search:Command
    {
        Axis axis;

        public Zero_Index_Search(Axis axis)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
            this.axis = axis;
        }
        public override string ToString()
        {
            return "AXE.ZERO " + AxisToInt(axis);
        }
    }
}
