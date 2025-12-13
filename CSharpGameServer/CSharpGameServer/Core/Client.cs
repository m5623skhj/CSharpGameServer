using System.Net.Sockets;
using CSharpGameServer.PacketBase;

namespace CSharpGameServer.Core
{
    public partial class Client(ServerCore inServerCore, Socket inSocket, ulong inClientSessionId)
    {
        protected ServerCore ServerCore = inServerCore;
        private const ulong InvalidSessionId = ulong.MaxValue;

        public Socket Socket { get; } = inSocket;
        public ulong ClientSessionId { get; private set; } = inClientSessionId;
        private readonly StreamRingBuffer streamRingBuffer = new();
        public DateTime LastReceivedTime { get; private set; } = DateTime.Now;

        public virtual void OnConnected()
        {
        }

        public virtual void OnClosed() 
        {
            ClientSessionId = InvalidSessionId;
        }

        public virtual void OnSend() {}

        public void CloseSession()
        {
            ServerCore.CloseClient(ClientSessionId);
        }

        public void RefreshRecvTime()
        {
            LastReceivedTime = DateTime.Now;
        }

        public void Send(ReplyPacket packet)
        {
            ServerCore.SendPacket(this, packet);
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
