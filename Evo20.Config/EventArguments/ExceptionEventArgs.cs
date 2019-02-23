using System;

namespace Evo20.Utils.EventArguments
{
    public class ExceptionEventArgs:EventArgs
    {
        public Exception Exception;

        public ExceptionEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
