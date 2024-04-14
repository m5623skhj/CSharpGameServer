using CSharpGameServer.Protocol;
using System.Net.Sockets;

namespace CSharpGameServer.Core
{
    public class Client
    {
        public Socket socket { get; }
        public ulong clientSessionId { get; }
        public StreamRingBuffer streamRingBuffer = new StreamRingBuffer();

        public Client(Socket inSocket, ulong inClientSessionId) 
        {
            socket = inSocket;
            clientSessionId = inClientSessionId;
        }

        public void CloseSession()
        {
            ServerCore.Instance.CloseClient(clientSessionId);
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

        public void PushStreamData(byte[] inputStreamData)
        {
            streamRingBuffer.PushData(inputStreamData);
        }

        public byte[]? PopStreamData(uint popSize)
        {
            return streamRingBuffer.PopData(popSize);
        }

        public byte[]? PeekStreamData(uint peekSize)
        {
            return streamRingBuffer.PeekData(peekSize);
        }
    }
}
