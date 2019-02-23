//using Evo20.Commands.Abstract;

//namespace Evo20.Commands.ControlCommands
//{
//    public class Frequency : ControlCommand
//    {
//        readonly Axis _axis;
//        readonly double _frequency;
//        public Frequency(Axis axis, double frequency)
//        {
//            _axis = axis;
//            _frequency = frequency;
//        }
//        public override string ToString()
//        {
//            string buffer = "AXE.CONS.FRE " + _frequency;
//            return buffer + ' ' + AxisToInt(_axis);
//        }
//    }
//}
