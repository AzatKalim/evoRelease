using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    public class Motor_torque_Query:Command
    {
        Axis axis;
        public Motor_torque_Query(Axis axis, double phase)
        {
            is_answer = false;
            have_answer = true;
            can_send = true;
            this.axis = axis;
        }
        public override string ToString()
        {
            string buffer = "AXE.TELL.CPL "+AxisToInt(axis);
            return buffer;
        }
    }
}
