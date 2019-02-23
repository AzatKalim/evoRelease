using Evo20.Commands.Abstract;

namespace Evo20.Commands.CommndsWithAnswer
{
    public class ActualTemperatureQuery : CommandWithAnswer
    {
        public static string Command = "CLIM.TEMP";

        public override string ToString()
        {
            string buffer = "CLIM.TEMP";
            return buffer;
        }
    }
}
