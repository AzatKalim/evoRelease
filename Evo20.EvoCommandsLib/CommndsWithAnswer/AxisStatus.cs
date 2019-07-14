using Evo20.Commands.Abstract;

namespace Evo20.Commands.CommndsWithAnswer
{
    public class AxisStatus : CommandWithAnswer
    {
        public static string Command = "AXE.TELL.STA";

        public override string ToString()
        {
            return "AXE.TELL.STA ";
        }
    }
}
