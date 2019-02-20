using System;

namespace Evo20.Utils.EventArguments
{
    public class BoolEventArgs : EventArgs
    {
        public bool result;
        public BoolEventArgs(bool result)
        {
            this.result = result;
        }
    }
}
