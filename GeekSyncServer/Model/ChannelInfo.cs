using System;
using GeekSyncServer.Internal;

namespace GeekSyncServer.Model
{
    public class ChannelInfo
    {
        public bool Active { get; }
        public string[] Desktops { get; }


        public ChannelInfo(Channel channel)
        {
            Active = channel != null;

            Desktops = Active ? channel.DesktopNames : new string[] { };

        }
    }

}

