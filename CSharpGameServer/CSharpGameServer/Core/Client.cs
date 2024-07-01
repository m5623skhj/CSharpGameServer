using CSharpGameServer.Protocol;
using System.Net.Sockets;

namespace CSharpGameServer.Core
{
    public class Client
    {
        public Socket socket { get; }
        public ulong clientSessionId { get; }
        private StreamRingBuffer streamRingBuffer = new StreamRingBuffer();
        public DateTime lastRecvedTime { get; private set; } = DateTime.Now;

        public virtual void OnClosed() {}
        public virtual void OnSend() {}

        public Client(Socket inSocket, ulong inClientSessionId) 
        {
            socket = inSocket;
            clientSessionId = inClientSessionId;
        }

        public void CloseSession()
        {
            ServerCore.Instance.CloseClient(clientSessionId);
        }

        public void RefreshRecvTime()
        {
            lastRecvedTime = DateTime.Now;
        }

        public void Send(ReplyPacket packet)
        {
            ServerCore.Instance.SendPacket(this, packet);
        }

        public bool PushStreamData(byte[] inputStreamData)
        {
            return streamRingBuffer.PushData(inputStreamData);
        }

        public byte[] PeekAllStreamData()
        {
            return streamRingBuffer.PeekAllData();
        }

        public void RemoveStreamData(uint removeSize)
        {
            streamRingBuffer.EraseData(removeSize);
        }
    }
}
