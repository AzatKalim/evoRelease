using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.Commands
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
