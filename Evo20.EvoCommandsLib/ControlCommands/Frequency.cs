using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    /// <summary>
    /// задать частоту вращения  заданной оси
    /// </summary>
    public class Frequency : ControlCommand
    {
        Axis axis;
        double frequency;
        public Frequency(Axis axis, double frequency)
        {
            this.axis = axis;
            this.frequency = frequency;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.FRE " + frequency;
            return buffer + ' ' + AxisToInt(axis);
        }
    }
}
