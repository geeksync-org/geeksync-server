using System;

namespace GeekSyncServer.Exceptions
{
    public class ChannelAlreadyExistsException:Exception
    {

        public ChannelAlreadyExistsException(string message) 
            : base (message)
        {
        }

        public ChannelAlreadyExistsException(string message,Exception inner) 
            : base (message,inner)
        {
        }
        public ChannelAlreadyExistsException() 
            : base ("Channel already Exists")
        {
        }

        public ChannelAlreadyExistsException(Exception inner) 
            : base ("Channel already Exists,inner")
        {
        }
    }
}