using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.Commands
{
    public class Rotary_joint_temperature_Query_answer:Command
    {
        public double temperture;
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
            temper = temper.Replace(',', '.');
            if (!double.TryParse(temper, out this.temperture))
            {
                temper = temper.Replace('.', ',');
                double.TryParse(temper, out this.temperture);
            }
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
