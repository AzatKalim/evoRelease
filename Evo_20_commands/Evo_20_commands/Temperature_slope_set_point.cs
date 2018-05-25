using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo_20_commands
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
