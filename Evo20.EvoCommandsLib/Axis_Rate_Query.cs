using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    public class Axis_Rate_Query:Command
    {
        Axis axis;
        public Axis_Rate_Query(Axis axis)
        {
            is_answer = false;
            have_answer = true;
            can_send = true;
            this.axis = axis;
        }
        public override string ToString()
        {
            string buffer = "AXE.TELL.VIT "+ AxisToInt(axis);
            return buffer;
        }
    }
}
