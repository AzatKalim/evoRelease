using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo_20_commands
{
    public class Brakes_lock:Command
    {
        Axis axis;
        bool _lock;
        public Brakes_lock(Axis axis, bool _lock)
        {
            is_answer = false;
            have_answer = false;
            can_send = true;
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
