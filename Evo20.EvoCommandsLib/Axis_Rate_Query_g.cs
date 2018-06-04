﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    public class Axis_Rate_Query_g:Command
    {
        Axis axis;
        public Axis_Rate_Query_g(Axis axis, double phase)
        {
            is_answer = false;
            have_answer = true;
            can_send = true;
            this.axis = axis;
        }
        public override string ToString()
        {
            string buffer = "AXE.TELL.VITG "+ axis;
            return buffer;
        }
    }
}
