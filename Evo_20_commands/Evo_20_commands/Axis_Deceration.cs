using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo_20_commands
{
    public class Axis_Deceleration : Command
    {
        Axis axis;
        double speed_of_braking;
        public Axis_Deceleration(Axis axis, double speed_of_braking)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
            this.axis = axis;
            this.speed_of_braking = speed_of_braking;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.DEC " + speed_of_braking;
            return buffer + ' ' + AxisToInt(axis);
        }
    }
}
