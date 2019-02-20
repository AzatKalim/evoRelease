using System;

namespace Evo20.Utils.EventArguments
{
    public class ExceptionEventArgs:EventArgs
    {
        public Exception exception;

        public ExceptionEventArgs(Exception exception)
        {
            this.exception = exception;
        }
    }
}
