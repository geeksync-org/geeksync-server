using System;

namespace GeekSyncServer.Exceptions
{
    public class DesktopNotConnectedException:Exception
    {

        public DesktopNotConnectedException(string message) 
            : base (message)
        {
        }

        public DesktopNotConnectedException(string message,Exception inner) 
            : base (message,inner)
        {
        }
        public DesktopNotConnectedException() 
            : base ("Desktop not connected.")
        {
        }

        public DesktopNotConnectedException(Exception inner) 
            : base ("Desktop not connected.")
        {
        }
    }
}