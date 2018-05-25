using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo_20_commands
{
    public class Axis_Acceleration:Command
    {
        Axis axis;
        double speed_of_acceleration;
        public Axis_Acceleration(Axis axis, double speed_of_acceleration)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
            this.axis = axis;
            this.speed_of_acceleration = speed_of_acceleration;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.ACC " +speed_of_acceleration;
            return buffer + ' ' + AxisToInt(axis);
        }
    }
}
