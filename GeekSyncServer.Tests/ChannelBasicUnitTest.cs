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
            Assert.Equal(validCh,channel.PairingID);
            Assert.NotEqual(invalidCh,channel.PairingID);
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
            Assert.Equal(validCh,channelManager[validCh].PairingID);
            Assert.Empty(channelManager[validCh].DesktopNames);
            channelManager.DeleteChannel(validCh);
            Assert.Null(channelManager[validCh]);
        }

        [Fact]
        public void DesktopLifeCycleTest()
        {
            Guid validCh=Guid.NewGuid();
            Guid validDesktop=Guid.NewGuid();
            string desktopName="TEST 1";
            ChannelManager channelManager=ChannelManager.Instance;
            
            Assert.Null(channelManager[validCh]); // make sure we start with nonexisting channel
            channelManager.CreateChannel(validCh);

            Channel channel=channelManager[validCh];
            
            Assert.DoesNotContain(desktopName,channel.DesktopNames);
            
            
            channel.ConnectDesktop(validDesktop,desktopName);
            
            //Assert.Equal(1,channel.DesktopNames.Length);
            Assert.Single(channel.DesktopNames);
            Assert.Contains(desktopName,channel.DesktopNames);

            channel.DisconnectDesktop(validDesktop);

            Assert.Null(channelManager[validCh]);
        }

        [Fact]
        public void ChannelWithTwoDesktopsLifeCycleTest()
        {
            Guid validCh=Guid.NewGuid();
            Guid validDesktop1=Guid.NewGuid();
            Guid validDesktop2=Guid.NewGuid();
            string desktopName1="TEST 1";
            string desktopName2="TEST 2";

            //Channel init
            
            ChannelManager channelManager=ChannelManager.Instance;
            Assert.NotNull(channelManager);
            Assert.Null(channelManager[validCh]);
            channelManager.CreateChannel(validCh);
            Assert.NotNull(channelManager[validCh]);
            Assert.Equal(validCh,channelManager[validCh].PairingID);
            Assert.Empty(channelManager[validCh].DesktopNames);
            Channel channel=channelManager[validCh];

            //Desktop 1 connect
            channel.ConnectDesktop(validDesktop1,desktopName1);
            
            Assert.Single(channel.DesktopNames);
            Assert.Contains(desktopName1,channel.DesktopNames);

             //Desktop 2 connect and disconnect
            channel.ConnectDesktop(validDesktop2,desktopName2);
            
            Assert.Contains(desktopName1,channel.DesktopNames);
            Assert.Contains(desktopName2,channel.DesktopNames);

            //Desktop 2 disconnect

            channel.DisconnectDesktop(validDesktop2);

            Assert.Same(channel,channelManager[validCh]);
            Assert.Contains(desktopName1,channel.DesktopNames);
            Assert.DoesNotContain(desktopName2,channel.DesktopNames);
            Assert.Single(channel.DesktopNames);

            //Desktop 1 disconnect

            channel.DisconnectDesktop(validDesktop1);
            Assert.Null(channelManager[validCh]);





        }


        

    }
}
