using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Emergency_stop : ControlCommand
    {
        public override string ToString()
        {
            string buffer = "AXE.AU ";
            return buffer;
        }
    }
}
