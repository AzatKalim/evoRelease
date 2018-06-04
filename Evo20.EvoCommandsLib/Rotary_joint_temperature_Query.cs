using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    /// <summary>
    /// Команда температурной камеры 
    /// </summary>
    public class Rotary_joint_temperature_Query:Command
    {
        Axis axis;
        public Rotary_joint_temperature_Query(Axis axis)
        {
            is_answer = false;
            have_answer = true;
            can_send = true;
            this.axis = axis;
        }
        public override string ToString()
        {
            string buffer = "AXE.TELL.TJT "+ AxisToInt(axis);
            return buffer;
        }
    }
}
