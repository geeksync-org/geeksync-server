using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using GeekSyncServer.Exceptions;
using System.Text;

namespace GeekSyncServer.Internal
{
    public class Channel
    {

        private WebSocket webSocket;

        public readonly Guid ChannelID;

        public Channel(Guid channelID)
        {
            this.ChannelID = channelID;
        }
        public async Task ConnectWebSocket(WebSocket webSocket)
        {
            if (webSocket!=null)
            {
                // so, we already have one. For now, we just close it...
                // TODO: check how to close...
                await this.webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,"Closing...",new CancellationTokenSource().Token);
            }
            this.webSocket=webSocket;
            try
            {
                var buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = await this.webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                while (!result.CloseStatus.HasValue)
                {
                    ///await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    result = await this.webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
            catch (Exception e)
            {
                //throw new DesktopWebSocketException(e);
                throw e;
            }
            

        }


        public async Task SendToReceiver(string message)
        {
            //TODO: implement true buffer size alignment and not limit to 4000 bytes!
            byte[] bytes=Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length>4000?4000:bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}