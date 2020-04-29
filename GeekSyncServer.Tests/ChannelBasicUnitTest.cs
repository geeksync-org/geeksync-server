using System;
using Xunit;
using GeekSyncServer.Internal;

namespace GeekSyncServer.Tests
{
    public class ChannelBasicUnitTest
    {
       

        
 
        [Fact]
        public void ChannelClassGuidTest()
        {
            Guid validCh=Guid.NewGuid();
            Guid invalidCh=Guid.NewGuid();
            Channel channel=new Channel(validCh);
            Assert.NotNull(channel);
            Assert.Equal(validCh,channel.ChannelID);
            Assert.NotEqual(invalidCh,channel.ChannelID);
        }

        [Fact]
        public void ChannelManagerChannelLifeCycleTest()
        {
            Guid validCh=Guid.NewGuid();
            ChannelManager channelManager=ChannelManager.Instance;
            Assert.NotNull(channelManager);
            Assert.Null(channelManager[validCh]);
            channelManager.CreateChannel(validCh);
            Assert.NotNull(channelManager[validCh]);
            Assert.Equal(validCh,channelManager[validCh].ChannelID);
            channelManager.DeleteChannel(validCh);
            Assert.Null(channelManager[validCh]);
        }

        


        

    }
}
