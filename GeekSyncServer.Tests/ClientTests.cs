using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System;
using System.Threading;
using GeekSyncClient.Client;
using System.Linq;

namespace GeekSyncClient.IntegrationTests
{
    public class ClientTests
        : IClassFixture<WebApplicationFactory<GeekSyncServer.Startup>>
    {
        private readonly WebApplicationFactory<GeekSyncServer.Startup> _factory;

        public ClientTests(WebApplicationFactory<GeekSyncServer.Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public void CheckRandomChannel()
        {
            Guid ch=Guid.NewGuid();
            var httpClient=_factory.CreateClient();

            ConfigManager config=new ConfigManager(".test.1.conf");

            SenderClient client=new SenderClient(config,config.Config.Peers[0].ChannelID,httpClient.BaseAddress.ToString(),httpClient);

            client.CheckIfAvailable();

            Assert.False(client.IsAvailable);

        }

        [Fact (Skip = "Need websocket support")]
        public void ChannelLifecycle()
        {
            
            Guid ch=Guid.NewGuid();
            var httpClient=_factory.CreateClient();

            ConfigManager config=new ConfigManager(".test.2.conf");

            SenderClient sender=new SenderClient(config,config.Config.Peers[0].ChannelID,httpClient.BaseAddress.ToString(),httpClient);
            ReceiverClient receiver=new ReceiverClient(config,config.Config.Peers[0].ChannelID,httpClient.BaseAddress.ToString(),httpClient);

            sender.CheckIfAvailable();
            Assert.False(sender.IsAvailable);

            receiver.Connect();

            sender.CheckIfAvailable();
            Assert.True(sender.IsAvailable);

            receiver.Disconnect();

            sender.CheckIfAvailable();
            Assert.False(sender.IsAvailable);

        }

        private string lastMsg="";

        void receiverHanlder(string msg)
        {
            lastMsg=msg;
        }

        [Fact (Skip = "Need websocket support")]
        public void SendAndReceive()
        {
           // Guid ch=Guid.NewGuid();
            var httpClient=_factory.CreateClient();

            Console.WriteLine("Using SVC URL: "+httpClient.BaseAddress.ToString());
            ConfigManager rconfig=new ConfigManager(".test.3r.conf");
            ConfigManager sconfig=new ConfigManager(".test.3s.conf");

            rconfig.PeerWith(sconfig);

            Guid ch=sconfig.Config.Peers.Single(x=>x.PeerID==rconfig.Config.MyID).ChannelID;

            SenderClient sender=new SenderClient(sconfig,ch,httpClient.BaseAddress.ToString(),httpClient);
            ReceiverClient receiver=new ReceiverClient(rconfig,ch,httpClient.BaseAddress.ToString(),httpClient);

            sender.CheckIfAvailable();
            Assert.False(sender.IsAvailable);

            lastMsg="";

            receiver.MessageReceived=receiverHanlder;

            receiver.Connect();
/*
            Console.WriteLine("Using WS URL: "+receiver.webSocketClient.Url.ToString());
            Console.WriteLine("  WS IsStarted: "+receiver.webSocketClient.IsStarted.ToString());
            Console.WriteLine("  WS IsRunning: "+receiver.webSocketClient.IsRunning.ToString());
            Console.WriteLine("  WS State: "+receiver.webSocketClient.NativeClient.State.ToString());
*/
            

            sender.CheckIfAvailable();
            Assert.True(sender.IsAvailable);

            Assert.Equal("",lastMsg);
            sender.SendMessage("TEST MESSAGE");

            Thread.Sleep(10*1000);

            Assert.Equal("TEST MESSAGE",lastMsg);

            receiver.Disconnect();

            sender.CheckIfAvailable();
            Assert.False(sender.IsAvailable);

        }

    }
}