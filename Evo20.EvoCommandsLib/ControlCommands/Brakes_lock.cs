using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Brakes_lock : ControlCommand
    {
        Axis axis;
        bool _lock;
        public Brakes_lock(Axis axis, bool _lock)
        {
            this.axis = axis;
            this._lock = _lock;
        }
        public override string ToString()
        {
            string buffer = "AXE.BRAK ";
            if (_lock)
                buffer += '1';
            else
                buffer += '0';
            return buffer + ' ' + AxisToInt(axis);
        }
    }
}
