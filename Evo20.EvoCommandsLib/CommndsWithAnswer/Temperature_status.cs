using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Temperature_status : CommandWithAnswer
    {
        public static string Command = "AXE.UPLD.STA";

        public override string ToString()
        {
            string buffer = "AXE.UPLD.STA ";
            return buffer;
        }
    }
}
