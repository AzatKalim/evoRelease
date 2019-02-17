using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    //отклонение по заданной оси
    public class Offset : ControlCommand
    {
        Axis axis;
        double degree;
        public Offset(Axis axis, double degree)
        {
            this.axis = axis;
            this.degree = degree;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.OFF " + degree;
            return buffer + ' ' + AxisToInt(axis);
        }
    }
}
