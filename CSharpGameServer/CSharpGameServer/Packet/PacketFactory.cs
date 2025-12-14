using System.Diagnostics.CodeAnalysis;
using CSharpGameServer.Core;
using CSharpGameServer.Logger;
using CSharpGameServer.PacketBase;

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
            Packet = inPacket;
            ResultType = inResultType;
            PacketLength = inPacketSize;
        }

        public readonly RequestPacket? Packet;
        public readonly PacketResultType ResultType;
        public readonly ushort PacketLength;
    }

    public class PacketFactory
    {
        private const int HeaderSize = 6;
        private readonly Dictionary<PacketType, Type> packetTypeDict = new();

        [field: AllowNull, MaybeNull]
        public static PacketFactory Instance
        {
            get
            {
                if (field != null)
                {
                    return field;
                }

                field = new PacketFactory();
                field.packetTypeDict.Clear();

                return field;
            }
        }

        public bool RegisterPacket(PacketType packetType, Type packetObjectType)
        {
            if (packetType == PacketType.InvalidPacketType)
            {
                LoggerManager.Instance.WriteLogFatal("Invalid packet type {packetType}", packetObjectType);
                return false;
            }

            if (!typeof(RequestPacket).IsAssignableFrom(packetObjectType))
            {
                LoggerManager.Instance.WriteLogFatal("Invalid packet object type {packetObjectType}", packetObjectType);
                return false;
            }

            if (packetTypeDict.TryAdd(packetType, packetObjectType))
            {
                return true;
            }

            LoggerManager.Instance.WriteLogFatal("Duplicated packet type {packetType} / {packetObjectType}", packetType, packetObjectType);
            return false;
        }

        public RequestPacketResult CreatePacket(byte[] buffer, int offset)
        {
            var remainingSize = buffer.Length - offset;
            if (remainingSize < HeaderSize)
            {
                return new RequestPacketResult(null, PacketResultType.IncompleteReceived);
            }

            var packetType = BitConverter.ToInt32(buffer, offset);
            if (packetTypeDict.TryGetValue((PacketType)packetType, out var packetObjectType) == false)
            {
                LoggerManager.Instance.WriteLogError("Invalid packet type {packetType}", packetType);
                return new RequestPacketResult(null, PacketResultType.InvalidReceivedData);
            }

            var packetLength = BitConverter.ToUInt16(buffer, offset + 4);
            if (packetLength > remainingSize)
            {
                return new RequestPacketResult(null, PacketResultType.IncompleteReceived);
            }

            if (packetLength > StreamRingBuffer.DefaultBufferSize)
            {
                return new RequestPacketResult(null, PacketResultType.InvalidReceivedData);
            }

            var packet = (RequestPacket)Activator.CreateInstance(packetObjectType)!;
            packet.LoadFromBytes(buffer, offset, packetLength);
            return new RequestPacketResult(packet, PacketResultType.Success, packetLength);
        }
    }
}
