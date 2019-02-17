using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Light_command : ControlCommand
    {
        bool light_mode;
        public Light_command(bool light_mode)
        {
            this.light_mode = light_mode;
        }
        public override string ToString()
        {
            string buffer = "CLIM.ECL" + light_mode;
            return buffer;
        }
    }
}
