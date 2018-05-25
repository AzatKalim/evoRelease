using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo_20_commands
{
    //отклонение по заданной оси
    public class Offset : Command
    {
        Axis axis;
        double degree;
        public Offset(Axis axis, double degree)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
            this.axis = axis;
            this.degree = degree;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.OFF " + degree;
            return buffer + ' ' + AxisToInt(axis);
        }
    }
}
