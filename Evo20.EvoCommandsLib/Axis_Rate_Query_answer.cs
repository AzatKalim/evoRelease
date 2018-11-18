using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.Commands
{
    public class Axis_Rate_Query_answer : Command
    {
        public double speedOfRate;
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
            speedOfRate = speedOfRate.Replace(',', '.');
            if (!double.TryParse(speedOfRate, out this.speedOfRate))
            {
                speedOfRate = speedOfRate.Replace('.', ',');
                double.TryParse(speedOfRate, out this.speedOfRate);
            }    
            is_answer = true;
            have_answer = false;
            can_send = false;
        }
    }
}
