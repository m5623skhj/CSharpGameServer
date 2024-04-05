using CSharpGameServer.Protocol;
using System.Net.Sockets;

namespace CSharpGameServer.Core
{
    public class Client
    {
        public Socket socket { get; }
        public ulong clientSessionId { get; }

        public Client(Socket inSocket, ulong inClientSessionId) 
        {
            socket = inSocket;
            clientSessionId = inClientSessionId;
        }

        public void OnClosed()
        {

        }

        public void Send(ReplyPacket packet)
        {
            ServerCore.Instance.SendPacket(this, packet);
        }

        public void OnSend()
        {

        }
    }
}
