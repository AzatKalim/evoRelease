using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.Commands
{
    public class Axis_Position_Query:Command
    {
        Axis axis;
        public  Axis_Position_Query(Axis axis)
        {
            is_answer = false;
            have_answer = true;
            can_send = true;
            this.axis = axis;
        }

        public override string ToString()
        {
            string buffer = "AXE.TELL.POS "+ AxisToInt(axis);
            return buffer;
        }
    }
}
