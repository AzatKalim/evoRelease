using System;
using System.Collections.Generic;
using System.Text;
using Evo20.Commands.Abstract;
namespace Evo20.Commands
{
    public class Actual_temperature_query : CommandWithAnswer
    {
        public static string Command = "CLIM.TEMP";

        public override string ToString()
        {
            string buffer = "CLIM.TEMP";
            return buffer;
        }
    }
}
