using Evo20.Commands.Abstract;

namespace Evo20.Commands.ControlCommands
{
    public enum ModeParam
    {
        Position = 0,
        Speed = 1,     
    }
    public class Mode : ControlCommand
    {
        private readonly ModeParam _parametr;
        readonly Axis _axis;
        public Mode(ModeParam param, Axis axis)
        {
            _parametr = param;
            _axis = axis;
        }       
        public override string ToString()
        {
            return $"AXE.CONS.MOD {(int) _parametr} {AxisToInt(_axis)}";
        }
    }
}
