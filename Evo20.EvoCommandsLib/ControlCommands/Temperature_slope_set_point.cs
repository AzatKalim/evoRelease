using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Temperature_slope_set_point : ControlCommand
    {
        double slope;
        public Temperature_slope_set_point(double slope)
        {
            this.slope = slope;
        }
        public override string ToString()
        {
            string buffer = "CLIM.RAMP " + slope;
            return buffer;
        }
    }
}
