using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.Commands
{
    /// <summary>
    /// задать частоту вращения  заданной оси
    /// </summary>
    public class Frequency:Command
    {
        Axis axis;
        double frequency;
        public Frequency(Axis axis, double frequency)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
            this.axis = axis;
            this.frequency = frequency;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.FRE " + frequency;
            return buffer + ' ' + AxisToInt(axis);
        }
    }
}
