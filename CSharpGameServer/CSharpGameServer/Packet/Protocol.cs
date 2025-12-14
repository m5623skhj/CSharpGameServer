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

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CreateRoomData
    {
        public int PacketType;
        public ushort PacketSize;
        public unsafe fixed byte RoomName[20];
    }

    public class CreateRoomPacket : RequestPacket
    {
        public CreateRoomData Data;

        public override void SetPacketType()
        {
            Type = PacketType.CreateRoom;
        }

        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandleCreateRoom;
        }

        public override void LoadFromBytes(byte[] buffer, int offset, ushort length)
        {
            var size = Marshal.SizeOf<CreateRoomData>();
            if (length < size)
            {
                Logger.LoggerManager.Instance.WriteLogError("CreateRoom packet length {length} < struct size {size}", length, size);
                return;
            }

            if (offset + size > buffer.Length)
            {
                Logger.LoggerManager.Instance.WriteLogError("CreateRoom buffer overflow: offset={offset}, size={size}, bufferLength={bufferLength}", offset, size, buffer.Length);
                return;
            }

            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(buffer, offset, ptr, size);
                Data = Marshal.PtrToStructure<CreateRoomData>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public override byte[] ToBytes()
        {
            var size = Marshal.SizeOf<CreateRoomData>();
            Data.PacketType = (int)PacketType.CreateRoom;
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
    public struct RoomCreatedData
    {
        public int PacketType;
        public ushort PacketSize;
        public ushort ErrorCode;
    }

    public class RoomCreatedPacket : ReplyPacket
    {
        public RoomCreatedData Data;

        public override void SetPacketType()
        {
            Type = PacketType.RoomCreated;
        }

        public override void LoadFromBytes(byte[] buffer, int offset, ushort length)
        {
            var size = Marshal.SizeOf<RoomCreatedData>();
            if (length < size)
            {
                Logger.LoggerManager.Instance.WriteLogError("RoomCreated packet length {length} < struct size {size}", length, size);
                return;
            }

            if (offset + size > buffer.Length)
            {
                Logger.LoggerManager.Instance.WriteLogError("RoomCreated buffer overflow: offset={offset}, size={size}, bufferLength={bufferLength}", offset, size, buffer.Length);
                return;
            }

            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(buffer, offset, ptr, size);
                Data = Marshal.PtrToStructure<RoomCreatedData>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public override byte[] ToBytes()
        {
            var size = Marshal.SizeOf<RoomCreatedData>();
            Data.PacketType = (int)PacketType.RoomCreated;
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
    public struct JoinRoomData
    {
        public int PacketType;
        public ushort PacketSize;
        public unsafe fixed byte RoomName[20];
    }

    public class JoinRoomPacket : RequestPacket
    {
        public JoinRoomData Data;

        public override void SetPacketType()
        {
            Type = PacketType.JoinRoom;
        }

        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandleJoinRoom;
        }

        public override void LoadFromBytes(byte[] buffer, int offset, ushort length)
        {
            var size = Marshal.SizeOf<JoinRoomData>();
            if (length < size)
            {
                Logger.LoggerManager.Instance.WriteLogError("JoinRoom packet length {length} < struct size {size}", length, size);
                return;
            }

            if (offset + size > buffer.Length)
            {
                Logger.LoggerManager.Instance.WriteLogError("JoinRoom buffer overflow: offset={offset}, size={size}, bufferLength={bufferLength}", offset, size, buffer.Length);
                return;
            }

            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(buffer, offset, ptr, size);
                Data = Marshal.PtrToStructure<JoinRoomData>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public override byte[] ToBytes()
        {
            var size = Marshal.SizeOf<JoinRoomData>();
            Data.PacketType = (int)PacketType.JoinRoom;
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
    public struct RoomJoinedData
    {
        public int PacketType;
        public ushort PacketSize;
        public ushort ErrorCode;
    }

    public class RoomJoinedPacket : ReplyPacket
    {
        public RoomJoinedData Data;

        public override void SetPacketType()
        {
            Type = PacketType.RoomJoined;
        }

        public override void LoadFromBytes(byte[] buffer, int offset, ushort length)
        {
            var size = Marshal.SizeOf<RoomJoinedData>();
            if (length < size)
            {
                Logger.LoggerManager.Instance.WriteLogError("RoomJoined packet length {length} < struct size {size}", length, size);
                return;
            }

            if (offset + size > buffer.Length)
            {
                Logger.LoggerManager.Instance.WriteLogError("RoomJoined buffer overflow: offset={offset}, size={size}, bufferLength={bufferLength}", offset, size, buffer.Length);
                return;
            }

            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(buffer, offset, ptr, size);
                Data = Marshal.PtrToStructure<RoomJoinedData>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public override byte[] ToBytes()
        {
            var size = Marshal.SizeOf<RoomJoinedData>();
            Data.PacketType = (int)PacketType.RoomJoined;
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
    public struct LeaveRoomData
    {
        public int PacketType;
        public ushort PacketSize;
    }

    public class LeaveRoomPacket : RequestPacket
    {
        public LeaveRoomData Data;

        public override void SetPacketType()
        {
            Type = PacketType.LeaveRoom;
        }

        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandleLeaveRoom;
        }

        public override void LoadFromBytes(byte[] buffer, int offset, ushort length)
        {
            var size = Marshal.SizeOf<LeaveRoomData>();
            if (length < size)
            {
                Logger.LoggerManager.Instance.WriteLogError("LeaveRoom packet length {length} < struct size {size}", length, size);
                return;
            }

            if (offset + size > buffer.Length)
            {
                Logger.LoggerManager.Instance.WriteLogError("LeaveRoom buffer overflow: offset={offset}, size={size}, bufferLength={bufferLength}", offset, size, buffer.Length);
                return;
            }

            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(buffer, offset, ptr, size);
                Data = Marshal.PtrToStructure<LeaveRoomData>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public override byte[] ToBytes()
        {
            var size = Marshal.SizeOf<LeaveRoomData>();
            Data.PacketType = (int)PacketType.LeaveRoom;
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
    public struct RoomLeftData
    {
        public int PacketType;
        public ushort PacketSize;
        public ushort ErrorCode;
    }

    public class RoomLeftPacket : ReplyPacket
    {
        public RoomLeftData Data;

        public override void SetPacketType()
        {
            Type = PacketType.RoomLeft;
        }

        public override void LoadFromBytes(byte[] buffer, int offset, ushort length)
        {
            var size = Marshal.SizeOf<RoomLeftData>();
            if (length < size)
            {
                Logger.LoggerManager.Instance.WriteLogError("RoomLeft packet length {length} < struct size {size}", length, size);
                return;
            }

            if (offset + size > buffer.Length)
            {
                Logger.LoggerManager.Instance.WriteLogError("RoomLeft buffer overflow: offset={offset}, size={size}, bufferLength={bufferLength}", offset, size, buffer.Length);
                return;
            }

            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(buffer, offset, ptr, size);
                Data = Marshal.PtrToStructure<RoomLeftData>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public override byte[] ToBytes()
        {
            var size = Marshal.SizeOf<RoomLeftData>();
            Data.PacketType = (int)PacketType.RoomLeft;
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
    public struct SendChatData
    {
        public int PacketType;
        public ushort PacketSize;
        public unsafe fixed byte Message[30];
    }

    public class SendChatPacket : RequestPacket
    {
        public SendChatData Data;

        public override void SetPacketType()
        {
            Type = PacketType.SendChat;
        }

        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandleSendChat;
        }

        public override void LoadFromBytes(byte[] buffer, int offset, ushort length)
        {
            var size = Marshal.SizeOf<SendChatData>();
            if (length < size)
            {
                Logger.LoggerManager.Instance.WriteLogError("SendChat packet length {length} < struct size {size}", length, size);
                return;
            }

            if (offset + size > buffer.Length)
            {
                Logger.LoggerManager.Instance.WriteLogError("SendChat buffer overflow: offset={offset}, size={size}, bufferLength={bufferLength}", offset, size, buffer.Length);
                return;
            }

            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(buffer, offset, ptr, size);
                Data = Marshal.PtrToStructure<SendChatData>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public override byte[] ToBytes()
        {
            var size = Marshal.SizeOf<SendChatData>();
            Data.PacketType = (int)PacketType.SendChat;
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
    public struct ChatMessageData
    {
        public int PacketType;
        public ushort PacketSize;
        public unsafe fixed byte Message[30];
    }

    public class ChatMessagePacket : ReplyPacket
    {
        public ChatMessageData Data;

        public override void SetPacketType()
        {
            Type = PacketType.ChatMessage;
        }

        public override void LoadFromBytes(byte[] buffer, int offset, ushort length)
        {
            var size = Marshal.SizeOf<ChatMessageData>();
            if (length < size)
            {
                Logger.LoggerManager.Instance.WriteLogError("ChatMessage packet length {length} < struct size {size}", length, size);
                return;
            }

            if (offset + size > buffer.Length)
            {
                Logger.LoggerManager.Instance.WriteLogError("ChatMessage buffer overflow: offset={offset}, size={size}, bufferLength={bufferLength}", offset, size, buffer.Length);
                return;
            }

            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(buffer, offset, ptr, size);
                Data = Marshal.PtrToStructure<ChatMessageData>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public override byte[] ToBytes()
        {
            var size = Marshal.SizeOf<ChatMessageData>();
            Data.PacketType = (int)PacketType.ChatMessage;
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
    public struct RoomListRequestData
    {
        public int PacketType;
        public ushort PacketSize;
    }

    public class RoomListRequestPacket : RequestPacket
    {
        public RoomListRequestData Data;

        public override void SetPacketType()
        {
            Type = PacketType.RoomListRequest;
        }

        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandleRoomListRequest;
        }

        public override void LoadFromBytes(byte[] buffer, int offset, ushort length)
        {
            var size = Marshal.SizeOf<RoomListRequestData>();
            if (length < size)
            {
                Logger.LoggerManager.Instance.WriteLogError("RoomListRequest packet length {length} < struct size {size}", length, size);
                return;
            }

            if (offset + size > buffer.Length)
            {
                Logger.LoggerManager.Instance.WriteLogError("RoomListRequest buffer overflow: offset={offset}, size={size}, bufferLength={bufferLength}", offset, size, buffer.Length);
                return;
            }

            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(buffer, offset, ptr, size);
                Data = Marshal.PtrToStructure<RoomListRequestData>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public override byte[] ToBytes()
        {
            var size = Marshal.SizeOf<RoomListRequestData>();
            Data.PacketType = (int)PacketType.RoomListRequest;
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
    public struct RoomListUpdateData
    {
        public int PacketType;
        public ushort PacketSize;
        public unsafe fixed byte Rooms[200];
    }

    public class RoomListUpdatePacket : ReplyPacket
    {
        public RoomListUpdateData Data;

        public override void SetPacketType()
        {
            Type = PacketType.RoomListUpdate;
        }

        public override void LoadFromBytes(byte[] buffer, int offset, ushort length)
        {
            var size = Marshal.SizeOf<RoomListUpdateData>();
            if (length < size)
            {
                Logger.LoggerManager.Instance.WriteLogError("RoomListUpdate packet length {length} < struct size {size}", length, size);
                return;
            }

            if (offset + size > buffer.Length)
            {
                Logger.LoggerManager.Instance.WriteLogError("RoomListUpdate buffer overflow: offset={offset}, size={size}, bufferLength={bufferLength}", offset, size, buffer.Length);
                return;
            }

            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(buffer, offset, ptr, size);
                Data = Marshal.PtrToStructure<RoomListUpdateData>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public override byte[] ToBytes()
        {
            var size = Marshal.SizeOf<RoomListUpdateData>();
            Data.PacketType = (int)PacketType.RoomListUpdate;
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
    public struct SetMyNameData
    {
        public int PacketType;
        public ushort PacketSize;
        public unsafe fixed byte Name[12];
    }

    public class SetMyNamePacket : RequestPacket
    {
        public SetMyNameData Data;

        public override void SetPacketType()
        {
            Type = PacketType.SetMyName;
        }

        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandleSetMyName;
        }

        public override void LoadFromBytes(byte[] buffer, int offset, ushort length)
        {
            var size = Marshal.SizeOf<SetMyNameData>();
            if (length < size)
            {
                Logger.LoggerManager.Instance.WriteLogError("SetMyName packet length {length} < struct size {size}", length, size);
                return;
            }

            if (offset + size > buffer.Length)
            {
                Logger.LoggerManager.Instance.WriteLogError("SetMyName buffer overflow: offset={offset}, size={size}, bufferLength={bufferLength}", offset, size, buffer.Length);
                return;
            }

            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(buffer, offset, ptr, size);
                Data = Marshal.PtrToStructure<SetMyNameData>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public override byte[] ToBytes()
        {
            var size = Marshal.SizeOf<SetMyNameData>();
            Data.PacketType = (int)PacketType.SetMyName;
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
    public struct SetMyNameResultData
    {
        public int PacketType;
        public ushort PacketSize;
        public ushort ErrorCode;
    }

    public class SetMyNameResultPacket : ReplyPacket
    {
        public SetMyNameResultData Data;

        public override void SetPacketType()
        {
            Type = PacketType.SetMyNameResult;
        }

        public override void LoadFromBytes(byte[] buffer, int offset, ushort length)
        {
            var size = Marshal.SizeOf<SetMyNameResultData>();
            if (length < size)
            {
                Logger.LoggerManager.Instance.WriteLogError("SetMyNameResult packet length {length} < struct size {size}", length, size);
                return;
            }

            if (offset + size > buffer.Length)
            {
                Logger.LoggerManager.Instance.WriteLogError("SetMyNameResult buffer overflow: offset={offset}, size={size}, bufferLength={bufferLength}", offset, size, buffer.Length);
                return;
            }

            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(buffer, offset, ptr, size);
                Data = Marshal.PtrToStructure<SetMyNameResultData>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public override byte[] ToBytes()
        {
            var size = Marshal.SizeOf<SetMyNameResultData>();
            Data.PacketType = (int)PacketType.SetMyNameResult;
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