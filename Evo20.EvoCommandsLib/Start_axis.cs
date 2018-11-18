using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.Commands
{
    public class Start_axis:Command
    {
        Axis axis;

        public Start_axis(Axis axis)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
            this.axis = axis;
        }
        public override string ToString()
        {
            return "AXE.STRT " + AxisToInt(axis);
        }
    }
}
