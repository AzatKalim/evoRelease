using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    public class Rotary_joint_temperature_Query_answer:Command
    {
        public double temperture
        {
            get;
            private set;
        }
        public Axis axis
        {
            get;
            private set;
        }
        public Rotary_joint_temperature_Query_answer()
        {
            have_answer = false;
            can_send = false;
        }
        public Rotary_joint_temperature_Query_answer(string temper,Axis ax)
        {
            temperture = Convert.ToDouble(temper);
            axis = ax;
            have_answer = false;
            can_send = false;
        }
        public override string ToString()
        {
            return "AXE.TELL.TJT";
        }
    }
}
