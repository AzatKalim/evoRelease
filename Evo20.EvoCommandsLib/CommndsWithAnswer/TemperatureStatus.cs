using Evo20.Commands.Abstract;

namespace Evo20.Commands.CommndsWithAnswer
{
    public class TemperatureStatus : CommandWithAnswer
    {
        public static string Command = "AXE.UPLD.STA";

        public override string ToString()
        {
            string buffer = "AXE.UPLD.STA ";
            return buffer;
        }
    }
}
