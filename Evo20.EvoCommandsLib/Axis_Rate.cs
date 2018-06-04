using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    public class Axis_Rate:Command
    {
        Axis axis;
        double speed_of_rate;
        public Axis_Rate(Axis axis, double speed_of_rate)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
            this.axis = axis;
            this.speed_of_rate = speed_of_rate;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.VIT " + speed_of_rate;         
            return buffer + ' ' + AxisToInt(axis);
        }
    }
}
