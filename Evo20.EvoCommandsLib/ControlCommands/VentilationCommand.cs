using Evo20.Commands.Abstract;

namespace Evo20.Commands.ControlCommands
{
    public class VentilationCommand : ControlCommand
    {
        readonly int _ventilationMode;
        public VentilationCommand(int ventilationMode)
        {
            _ventilationMode = ventilationMode;
        }
        public override string ToString()
        {
            string buffer = "CLIM.VENT" + _ventilationMode;
            return buffer;
        }
    }
}
