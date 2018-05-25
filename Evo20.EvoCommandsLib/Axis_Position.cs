using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    public class Axis_Position:Command
    {
        Axis axis;
        double degree;
        public Axis_Position(Axis axis, double degree)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
            this.axis = axis;
            this.degree = degree;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.POS "+degree;         
            return buffer + ' ' + AxisToInt(axis);
        }
    }
}
