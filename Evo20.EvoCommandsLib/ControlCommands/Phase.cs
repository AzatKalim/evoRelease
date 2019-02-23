//using Evo20.Commands.Abstract;

//namespace Evo20.Commands.ControlCommands
//{
//    public class Phase : ControlCommand
//    {
//        readonly Axis _axis;
//        // в градусах
//        readonly double _phase;
//        public Phase(Axis axis, double phase)
//        {
//            _axis = axis;
//            _phase = phase;
//        }
//        public override string ToString()
//        {
//            string buffer = "AXE.CONS.PHA " + _phase;
//            return buffer + ' ' + AxisToInt(_axis);
//        }
//    }
//}
