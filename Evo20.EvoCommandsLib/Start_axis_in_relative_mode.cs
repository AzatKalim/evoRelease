using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    public class Start_axis_in_relative_mode:Command
    {
        Axis axis;

        public Start_axis_in_relative_mode(Axis axis)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
            this.axis = axis;
        }
        public override string ToString()
        {
            return "AXE.STREEL " + AxisToInt(axis);
        }
    }
}
