using System;
using System.Linq;
using GeekSyncServer.Exceptions;
using System.Collections.Generic;

namespace GeekSyncServer.Internal
{
    public sealed class ChannelManager
    {

        private static readonly Lazy<ChannelManager>
            lazy =
            new Lazy<ChannelManager>
                (() => new ChannelManager());
        public static ChannelManager Instance { get { return lazy.Value; } }

        private List<Channel> channelList = new List<Channel>();

        private ChannelManager()
        {
        }

        public Channel this[Guid index]
        {
            get
            {
                return channelList.SingleOrDefault(x => x.ChannelID == index);
            }
        }



        public Channel CreateChannel(Guid channelID)
        {
            if (this[channelID]==null)
            {
                Channel n=new Channel(channelID);
                channelList.Add(n);
                return n;
            }
            else
            {
                throw new ChannelAlreadyExistsException();
            }
        }

        public void DeleteChannel(Guid channelID)
        {
            Channel inst=this[channelID];
            if (inst!=null) DeleteChannel(inst);
        }

        public void DeleteChannel(Channel channel)
        {
            channelList.Remove(channel);
        }
    }
}
