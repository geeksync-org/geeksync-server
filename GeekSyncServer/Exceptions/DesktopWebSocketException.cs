using System;

namespace GeekSyncServer.Exceptions
{
    public class DesktopWebSocketException:Exception
    {

        public DesktopWebSocketException(string message) 
            : base (message)
        {
        }

        public DesktopWebSocketException(string message,Exception inner) 
            : base (message,inner)
        {
        }
        public DesktopWebSocketException() 
            : base ("Desktop WebSocket already connected.")
        {
        }

        public DesktopWebSocketException(Exception inner) 
            : base ("Desktop WebSocket exception.")
        {
        }
    }
}