using System.Net.Sockets;
using CSharpGameServer.PacketBase;

namespace CSharpGameServer.Core
{
    public partial class Client
    {
        protected ServerCore serverCore;
        public static ulong invalidSessionId = ulong.MaxValue;

        public Socket socket { get; }
        public ulong clientSessionId;
        private StreamRingBuffer streamRingBuffer = new StreamRingBuffer();
        public DateTime lastReceivedTime { get; private set; } = DateTime.Now;

        public virtual void OnClosed() 
        {
            clientSessionId = invalidSessionId;
        }

        public virtual void OnSend() {}

        public Client(ServerCore inServerCore, Socket inSocket, ulong inClientSessionId) 
        {
            serverCore = inServerCore;
            socket = inSocket;
            clientSessionId = inClientSessionId;
        }

        public void CloseSession()
        {
            serverCore.CloseClient(clientSessionId);
        }

        public void RefreshRecvTime()
        {
            lastReceivedTime = DateTime.Now;
        }

        public void Send(ReplyPacket packet)
        {
            serverCore.SendPacket(this, packet);
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
