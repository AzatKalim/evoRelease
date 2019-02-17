using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Axis_Rate_Query_g : CommandWithAnswer
    {
        Axis axis;
        public Axis_Rate_Query_g(Axis axis, double phase)
        {
            this.axis = axis;
        }
        public override string ToString()
        {
            string buffer = "AXE.TELL.VITG "+ axis;
            return buffer;
        }
    }
}
