using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    class Axis_Deceleration:Command
    {
        Axis axis;
        double speed_of_braking;
        public Axis_Deceleration(Axis value, double speed_of_braking)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
            axis = value;
            this.speed_of_braking = speed_of_braking;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.DEC " + speed_of_braking;
            return buffer + ' ' + axis;
        }
    }
}
