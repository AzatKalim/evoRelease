using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo_20_commands
{
    /// <summary>
    /// задать амплитуду
    /// </summary>
    public class Amplitude: Command
    {
        Axis axis;
        // в градусах
        double amplitude;
        public Amplitude(Axis value, double amplitude)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
            axis = value;
            this.amplitude = amplitude;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.AMP " + amplitude;
            return buffer + ' ' + AxisToInt(axis);;
        }
    }
}
