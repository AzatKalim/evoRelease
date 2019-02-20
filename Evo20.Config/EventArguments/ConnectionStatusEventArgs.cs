using System;

namespace Evo20.Utils.EventArguments
{
    public class ConnectionStatusEventArgs : EventArgs
    {
        public ConnectionStatus state;
        public ConnectionStatusEventArgs(ConnectionStatus state)
        {
            this.state = state;
        }
    }  
}
