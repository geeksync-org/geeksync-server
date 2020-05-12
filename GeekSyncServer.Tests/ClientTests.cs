using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System;
using System.Threading;
using GeekSyncClient.Client;

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

            SenderClient client=new SenderClient(ch,httpClient.BaseAddress.ToString(),httpClient);

            client.CheckIfAvailable();

            Assert.False(client.IsAvailable);

        }

        [Fact]
        public void ChannelLifecycle()
        {
            Guid ch=Guid.NewGuid();
            var httpClient=_factory.CreateClient();

            SenderClient sender=new SenderClient(ch,httpClient.BaseAddress.ToString(),httpClient);
            ReceiverClient receiver=new ReceiverClient(ch,httpClient.BaseAddress.ToString(),httpClient);

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

        [Fact]
        public void SendAndReceive()
        {
            Guid ch=Guid.NewGuid();
            var httpClient=_factory.CreateClient();

            Console.WriteLine("Using SVC URL: "+httpClient.BaseAddress.ToString());

            SenderClient sender=new SenderClient(ch,httpClient.BaseAddress.ToString(),httpClient);
            ReceiverClient receiver=new ReceiverClient(ch,httpClient.BaseAddress.ToString(),httpClient);

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