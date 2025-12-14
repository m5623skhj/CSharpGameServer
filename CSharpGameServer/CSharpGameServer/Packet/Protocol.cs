#nullable disable

using CSharpGameServer.Core;
using CSharpGameServer.PacketBase;
using System.Runtime.InteropServices;

namespace CSharpGameServer.Packet
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PingData
    {
        public int PacketType;
        public ushort PacketSize;
    }

    public class PingPacket : RequestPacket
    {
        public PingData Data;

        public override void SetPacketType()
        {
            Type = PacketType.Ping;
        }

        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandlePing;
        }

        public override void LoadFromBytes(byte[] buffer, int offset, ushort length)
        {
            var size = Marshal.SizeOf<PingData>();
            if (length < size)
            {
                Logger.LoggerManager.Instance.WriteLogError("Ping packet length {length} < struct size {size}", length, size);
                return;
            }

            if (offset + size > buffer.Length)
            {
                Logger.LoggerManager.Instance.WriteLogError("Ping buffer overflow: offset={offset}, size={size}, bufferLength={bufferLength}", offset, size, buffer.Length);
                return;
            }

            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(buffer, offset, ptr, size);
                Data = Marshal.PtrToStructure<PingData>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public override byte[] ToBytes()
        {
            var size = Marshal.SizeOf<PingData>();
            Data.PacketType = (int)PacketType.Ping;
            Data.PacketSize = (ushort)size;

            var buffer = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(Data, ptr, false);
                Marshal.Copy(ptr, buffer, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return buffer;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PongData
    {
        public int PacketType;
        public ushort PacketSize;
    }

    public class PongPacket : ReplyPacket
    {
        public PongData Data;

        public override void SetPacketType()
        {
            Type = PacketType.Pong;
        }

        public override void LoadFromBytes(byte[] buffer, int offset, ushort length)
        {
            var size = Marshal.SizeOf<PongData>();
            if (length < size)
            {
                Logger.LoggerManager.Instance.WriteLogError("Pong packet length {length} < struct size {size}", length, size);
                return;
            }

            if (offset + size > buffer.Length)
            {
                Logger.LoggerManager.Instance.WriteLogError("Pong buffer overflow: offset={offset}, size={size}, bufferLength={bufferLength}", offset, size, buffer.Length);
                return;
            }

            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(buffer, offset, ptr, size);
                Data = Marshal.PtrToStructure<PongData>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public override byte[] ToBytes()
        {
            var size = Marshal.SizeOf<PongData>();
            Data.PacketType = (int)PacketType.Pong;
            Data.PacketSize = (ushort)size;

            var buffer = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(Data, ptr, false);
                Marshal.Copy(ptr, buffer, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return buffer;
        }
    }

}