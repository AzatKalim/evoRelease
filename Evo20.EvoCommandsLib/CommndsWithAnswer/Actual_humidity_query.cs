using System;
using System.Collections.Generic;
using System.Text;
using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Actual_humidity_query : CommandWithAnswer
    {
        public override string ToString()
        {
            string buffer = "CLIM.HUM";
            return buffer;
        }
    }
}
