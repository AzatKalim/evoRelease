using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    /// <summary>
    /// задать амплитуду
    /// </summary>
    public class Amplitude: ControlCommand
    {
        Axis axis;
        // в градусах
        double amplitude;
        public Amplitude(Axis value, double amplitude)
        {
            axis = value;
            this.amplitude = amplitude;
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.AMP " + amplitude;
            return buffer + ' ' + AxisToInt(axis);;
        }
    }
}
