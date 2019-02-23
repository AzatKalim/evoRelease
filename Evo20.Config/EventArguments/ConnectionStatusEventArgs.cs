using System;

namespace Evo20.Utils.EventArguments
{
    public class ConnectionStatusEventArgs : EventArgs
    {
        public ConnectionStatus State;
        public ConnectionStatusEventArgs(ConnectionStatus state)
        {
            State = state;
        }
    }  
}
