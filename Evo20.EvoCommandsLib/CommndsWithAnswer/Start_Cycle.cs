using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Start_Cycle : CommandWithAnswer
    {
        string cycle_name;
        public Start_Cycle(string cycle_name)
        {
            this.cycle_name = cycle_name;
        }
        public override string ToString()
        {
            string buffer = "CLIM.CYCL" + cycle_name;
            return buffer;
        }
    }
}
