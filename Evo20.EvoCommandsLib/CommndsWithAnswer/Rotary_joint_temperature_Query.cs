using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    /// <summary>
    /// Команда температурной камеры 
    /// </summary>
    public class Rotary_joint_temperature_Query : CommandWithAnswer
    {
        Axis axis;
        public Rotary_joint_temperature_Query(Axis axis)
        {
            this.axis = axis;
        }
        public override string ToString()
        {
            string buffer = "AXE.TELL.TJT "+ AxisToInt(axis);
            return buffer;
        }
    }
}
