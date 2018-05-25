using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo_20_commands
{
    public class Axis_Rate_Query_answer : Command
    {
        public double speedOfRate
        {
            get;
            private set;
        }
        public Axis axis
        {
            get;
            private set;
        }
        public Axis_Rate_Query_answer()
        {
            is_answer = true;
            have_answer = false;
            can_send = false;
        }
        public Axis_Rate_Query_answer(String speedOfRate, Axis axis)
        {
            this.axis = axis;
            this.speedOfRate = Convert.ToDouble(speedOfRate);
            is_answer = true;
            have_answer = false;
            can_send = false;
        }
    }
}
