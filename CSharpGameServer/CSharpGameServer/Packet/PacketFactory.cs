using System.Runtime.InteropServices;
using System.Text;
using CSharpGameServer.Core;
using CSharpGameServer.Logger;
using CSharpGameServer.Protocol;

namespace CSharpGameServer.Packet
{
    public enum PacketResultType : short
    {
        Success = 0,
        IncompleteReceived,
        InvalidReceivedData,
    }

    public struct RequestPacketResult
    {
        public RequestPacketResult(RequestPacket? inPacket, PacketResultType inResultType, ushort inPacketSize = 0)
        {
            packet = inPacket;
            resultType = inResultType;
            packetLength = inPacketSize;
        }

        public readonly RequestPacket? packet = null;
        public readonly PacketResultType resultType = PacketResultType.Success;
        public readonly ushort packetLength = 0;
    }

    public class PacketFactory
    {
        // Packet header : PacketType(4) + PacketSize(2)
        private readonly int headerSize = 6;

        private static PacketFactory? instance = null;
        private readonly Dictionary<PacketType, Type> packetTypeDict = new Dictionary<PacketType, Type>();

        public static PacketFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PacketFactory();
                    instance.packetTypeDict.Clear();
                }

                return instance;
            }
        }

        public bool RegisterPacket(PacketType packetType, Type packetObjectType)
        {
            if (packetType == PacketType.InvalidPacketType)
            {
                LoggerManager.Instance.WriteLogFatal("Invalid packet type {packetType}", packetObjectType);
                return false;
            }

            if (packetTypeDict.ContainsKey(packetType))
            {
                LoggerManager.Instance.WriteLogFatal("Duplicated packet type {packetType} / {packetObjectType}", packetType, packetObjectType);
                return false;
            }

            packetTypeDict[packetType] = packetObjectType;
            return true;
        }

        public RequestPacketResult CreatePacket(string receivedData)
        {
            if (receivedData.Length < headerSize)
            {
                return new RequestPacketResult(null, PacketResultType.IncompleteReceived);
            }

            int.TryParse(receivedData.Substring(0, 4), out int packetType);
            if (packetTypeDict.TryGetValue((PacketType)packetType, out Type? packetObjectType) == false)
            {
                LoggerManager.Instance.WriteLogError("Invalid packet type {packetType}", packetType);
                return new RequestPacketResult(null, PacketResultType.InvalidReceivedData);
            }

            ushort.TryParse(receivedData.Substring(4, 2), out ushort packetLength);
            if (packetLength > receivedData.Length)
            {
                return new RequestPacketResult(null, PacketResultType.IncompleteReceived);
            }
            else if (packetLength > StreamRingBuffer.defaultBufferSize)
            {
                return new RequestPacketResult(null, PacketResultType.InvalidReceivedData);
            }

            if (typeof(RequestPacket).IsAssignableFrom(packetObjectType) == false)
            {
                LoggerManager.Instance.WriteLogError("Packet type {packetType} is valid but is not assignable", packetType);
                return new RequestPacketResult(null, PacketResultType.InvalidReceivedData);
            }

            byte[] recvStream = Encoding.UTF8.GetBytes(receivedData.Substring(0, packetLength));
            RequestPacket? packet = ToStr(recvStream, packetObjectType) as RequestPacket;
            if (packet == null)
            {
                LoggerManager.Instance.WriteLogError("Null RequestPacket / packet type {packetType}", packetType);
                return new RequestPacketResult(null, PacketResultType.InvalidReceivedData);
            }

            return new RequestPacketResult(packet, PacketResultType.Success, packetLength);
        }

        private object? ToStr(byte[] data, Type type)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            object? result = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), type);
            handle.Free();
            return result;
        }
    }
}
