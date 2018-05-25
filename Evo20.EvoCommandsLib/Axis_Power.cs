using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    public class Axis_Power : Command
    {
        Axis axis;
        bool control;
        public Axis_Power(Axis axis, bool control)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
            this.axis = axis;
            this.control = control;
        }
        public override string ToString()
        {
            string buffer = "AXE.PWR ";
            if (control)
                buffer += '1';
            else
                buffer += '0';
            return buffer + ' ' + AxisToInt(axis);
        }
    }
}
