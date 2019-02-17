using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Ventilation_command : ControlCommand
    {
        int ventilation_mode;
        public Ventilation_command(int ventilation_mode)
        {
            this.ventilation_mode = ventilation_mode;
        }
        public override string ToString()
        {
            string buffer = "CLIM.VENT" + ventilation_mode;
            return buffer;
        }
    }
}
