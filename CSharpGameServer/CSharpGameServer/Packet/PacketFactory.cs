using System.Runtime.InteropServices;
using System.Text;
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

        public RequestPacketResult CreatePacket(byte[] buffer, int offset)
        {
            var remainingSize = buffer.Length - offset;
            if (remainingSize < headerSize)
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

            if (packetLength > StreamRingBuffer.defaultBufferSize)
            {
                return new RequestPacketResult(null, PacketResultType.InvalidReceivedData);
            }

            if (typeof(RequestPacket).IsAssignableFrom(packetObjectType) == false)
            {
                LoggerManager.Instance.WriteLogError("Packet type {packetType} is valid but is not assignable", packetType);
                return new RequestPacketResult(null, PacketResultType.InvalidReceivedData);
            }

            if (BytesToStruct(buffer, offset, packetObjectType) is RequestPacket packet)
            {
                return new RequestPacketResult(packet, PacketResultType.Success, packetLength);
            }

            LoggerManager.Instance.WriteLogError("Null RequestPacket / packet type {packetType}", packetType);
            return new RequestPacketResult(null, PacketResultType.InvalidReceivedData);
        }

        private static object? BytesToStruct(byte[] data, int offset, Type structType)
        {
            var size = Marshal.SizeOf(structType);
            var ptr = Marshal.AllocHGlobal(size);
            
            try
            {
                Marshal.Copy(data, offset, ptr, size);
                return Marshal.PtrToStructure(ptr, structType);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
