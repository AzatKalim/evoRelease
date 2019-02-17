using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Requested_axis_position_reached : CommandWithAnswer
    {
        public static string Command = "AXE.TELL.POA";

        public override string ToString()
        {
            string buffer = "AXE.TELL.POA";
            return buffer;
        }
    }
}
