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

        private Dictionary<Guid, string> Desktops = new Dictionary<Guid, string>();
        private Dictionary<Guid, WebSocket> Sockets = new Dictionary<Guid, WebSocket>();

        public readonly Guid PairingID;

        public Channel(Guid pairingID)
        {
            this.PairingID = pairingID;
        }

        public string[] DesktopNames
        {
            get
            {
                return Desktops.Values.ToArray();
            }
        }

        public async Task ConnectWebSocket(Guid desktopID, WebSocket webSocket)
        {
            if (!Desktops.ContainsKey(desktopID)) throw new DesktopNotConnectedException();
            if (Sockets.ContainsKey(desktopID)) throw new DesktopWebSocketAlreadyConnectedException();

            Sockets.Add(desktopID, webSocket);
            try
            {
                var buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                while (!result.CloseStatus.HasValue)
                {
                    ///await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
            catch (Exception e)
            {
                throw new DesktopWebSocketException(e);
            }
            finally
            {
                Sockets.Remove(desktopID);
            }

        }

        public void ConnectDesktop(Guid desktopId, string desktopName)
        {
            if (Desktops.ContainsKey(desktopId))
            {
                Desktops[desktopId] = desktopName;
            }
            else
            {
                Desktops.Add(desktopId, desktopName);
            }
        }


        public void DisconnectDesktop(Guid desktopId)
        {
            if (!Desktops.ContainsKey(desktopId))
            {
                return;
            }
            else
            {
                Desktops.Remove(desktopId);
                if (Desktops.Count == 0)
                {
                    ChannelManager.Instance.DeleteChannel(this);
                }
            }
        }

        public async Task SentToAllDesktops(string message)
        {
            //TODO: Do it in true async!!!
            foreach (Guid k in Sockets.Keys)
            {
                await SendToDesktop(k,message);
            }
            
        }

        public async Task SendToDesktop(Guid desktopID,string message)
        {
            //TODO: implement true buffer size alignment and not limit to 4000 bytes!
            byte[] bytes=Encoding.UTF8.GetBytes(message);
            await Sockets[desktopID].SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length>4000?4000:bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}