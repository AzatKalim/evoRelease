using System;

namespace Evo20.Utils.EventArguments
{
    public class WorkModeEventArgs : EventArgs
    {
        public WorkMode mode;

        public WorkModeEventArgs(WorkMode mode)
        {
            this.mode = mode;
        }
    }
}
