﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo_20_commands
{
    public class Start_axis:Command
    {
        Axis axis;

        public Start_axis(Axis axis)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
            this.axis = axis;
        }
        public override string ToString()
        {
            return "AXE.STRT " + AxisToInt(axis);
        }
    }
}
