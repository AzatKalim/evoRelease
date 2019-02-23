using System;

namespace Evo20.Utils.EventArguments
{
    public class BoolEventArgs : EventArgs
    {
        public bool Result;
        public BoolEventArgs(bool result)
        {
            Result = result;
        }
    }
}
