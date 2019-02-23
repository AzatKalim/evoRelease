//using Evo20.Commands.Abstract;

//namespace Evo20.Commands.ControlCommands
//{
//    public class BrakesLock : ControlCommand
//    {
//        readonly Axis _axis;
//        readonly bool _lock;
//        public BrakesLock(Axis axis, bool _lock)
//        {
//            _axis = axis;
//            this._lock = _lock;
//        }
//        public override string ToString()
//        {
//            string buffer = "AXE.BRAK ";
//            if (_lock)
//                buffer += '1';
//            else
//                buffer += '0';
//            return buffer + ' ' + AxisToInt(_axis);
//        }
//    }
//}
