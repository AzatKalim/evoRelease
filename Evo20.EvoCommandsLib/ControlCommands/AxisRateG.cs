//using Evo20.Commands.Abstract;

//namespace Evo20.Commands.ControlCommands
//{
//    public class AxisRateG : ControlCommand
//    {
//        readonly Axis _axis;
//        readonly double _speedOfRateInG;
//        public AxisRateG(Axis axis, double speedOfRate)
//        {
//            _axis = axis;
//            _speedOfRateInG = speedOfRate;
//        }
//        public override string ToString()
//        {
//            string buffer = "AXE.CONS.VITG " + _speedOfRateInG;
//            return buffer + ' ' + AxisToInt(_axis);
//        }
//    }
//}
