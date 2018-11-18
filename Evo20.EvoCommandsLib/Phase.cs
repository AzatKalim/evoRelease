using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.Commands
{
    public class Phase:Command
    {
        Axis axis;
        // в градусах
        double phase;
        public Phase(Axis axis, double phase)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
            this.axis = axis;
            this.phase = phase;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.PHA " + phase;
            return buffer + ' ' + AxisToInt(axis);
        }
    }
}
