using System;

namespace GeekSyncServer.Exceptions
{
    public class DesktopWebSocketAlreadyConnectedException:Exception
    {

        public DesktopWebSocketAlreadyConnectedException(string message) 
            : base (message)
        {
        }

        public DesktopWebSocketAlreadyConnectedException(string message,Exception inner) 
            : base (message,inner)
        {
        }
        public DesktopWebSocketAlreadyConnectedException() 
            : base ("Desktop WebSocket already connected.")
        {
        }

        public DesktopWebSocketAlreadyConnectedException(Exception inner) 
            : base ("Desktop WebSocket already connected.")
        {
        }
    }
}