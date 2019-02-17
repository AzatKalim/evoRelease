using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Temperature_Set_point : ControlCommand
    {
        double temperature;
        public Temperature_Set_point(double temperature)
        {
            this.temperature = temperature;
        }
        public override string ToString()
        {
            string buffer = "CLIM.CONS " + temperature;
            return buffer;
        }
    }
}
