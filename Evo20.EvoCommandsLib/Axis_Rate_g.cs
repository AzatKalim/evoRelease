using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.Commands
{
    public class Axis_Rate_g : Command
    {
        Axis axis;
        double speed_of_rate_in_g;
        public Axis_Rate_g(Axis axis, double speed_of_rate)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
            this.axis = axis;
            this.speed_of_rate_in_g = speed_of_rate;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.VITG " + speed_of_rate_in_g;
            return buffer + ' ' + AxisToInt(axis);
        }
    }
}
