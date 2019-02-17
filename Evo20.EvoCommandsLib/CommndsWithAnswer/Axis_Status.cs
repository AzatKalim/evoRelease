using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Axis_Status : CommandWithAnswer
    {
        public static string Command = "AXE.TELL.STA";

        public override string ToString()
        {
            string buffer = "AXE.TELL.STA ";
            return buffer;
        }
    }
}
