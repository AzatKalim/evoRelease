using System;

namespace Evo20.Utils.EventArguments
{
    public class WorkModeEventArgs : EventArgs
    {
        public WorkMode Mode;

        public WorkModeEventArgs(WorkMode mode)
        {
            Mode = mode;
        }
    }
}
