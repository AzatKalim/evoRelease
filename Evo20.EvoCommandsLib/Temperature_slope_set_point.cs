using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.Commands
{
    public class Temperature_slope_set_point:Command
    {
        double slope;
        public Temperature_slope_set_point(double slope)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
            this.slope = slope;
        }
        public override string ToString()
        {
            string buffer = "CLIM.RAMP " + slope;
            return buffer;
        }
    }
}
