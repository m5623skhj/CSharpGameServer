#nullable disable

using CSharpGameServer.Core;
using CSharpGameServer.PacketBase;
using System.Runtime.InteropServices;

namespace CSharpGameServer.Packet
{
    public class PingPacket : RequestPacket
    {
        public override void SetPacketType()
        {
            Type = PacketType.Ping;
        }
        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandlePing;
        }
    }

    public class PongPacket : ReplyPacket
    {
        public override void SetPacketType()
        {
            Type = PacketType.Pong;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CreateRoom
    {
        public string RoomName { get; set; }
    }
    public class CreateRoomPacket : RequestPacket
    {
        public CreateRoom Data { get; set; }
        public override void SetPacketType()
        {
            Type = PacketType.CreateRoom;
        }
        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandleCreateRoom;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RoomCreated
    {
        public ushort ErrorCode { get; set; }
    }
    public class RoomCreatedPacket : ReplyPacket
    {
        public RoomCreated Data { get; set; }
        public override void SetPacketType()
        {
            Type = PacketType.RoomCreated;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct JoinRoom
    {
        public string RoomName { get; set; }
    }
    public class JoinRoomPacket : RequestPacket
    {
        public JoinRoom Data { get; set; }
        public override void SetPacketType()
        {
            Type = PacketType.JoinRoom;
        }
        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandleJoinRoom;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RoomJoined
    {
        public ushort ErrorCode { get; set; }
    }
    public class RoomJoinedPacket : ReplyPacket
    {
        public RoomJoined Data { get; set; }
        public override void SetPacketType()
        {
            Type = PacketType.RoomJoined;
        }
    }

    public class LeaveRoomPacket : RequestPacket
    {
        public override void SetPacketType()
        {
            Type = PacketType.LeaveRoom;
        }
        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandleLeaveRoom;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RoomLeft
    {
        public ushort ErrorCode { get; set; }
    }
    public class RoomLeftPacket : ReplyPacket
    {
        public RoomLeft Data { get; set; }
        public override void SetPacketType()
        {
            Type = PacketType.RoomLeft;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SendChat
    {
        public string Message { get; set; }
    }
    public class SendChatPacket : RequestPacket
    {
        public SendChat Data { get; set; }
        public override void SetPacketType()
        {
            Type = PacketType.SendChat;
        }
        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandleSendChat;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ChatMessage
    {
        public string Message { get; set; }
    }
    public class ChatMessagePacket : ReplyPacket
    {
        public ChatMessage Data { get; set; }
        public override void SetPacketType()
        {
            Type = PacketType.ChatMessage;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RoomListUpdate
    {
        public string[] Rooms { get; set; }
    }
    public class RoomListUpdatePacket : ReplyPacket
    {
        public RoomListUpdate Data { get; set; }
        public override void SetPacketType()
        {
            Type = PacketType.RoomListUpdate;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SetMyName
    {
        public string Name { get; set; }
    }
    public class SetMyNamePacket : RequestPacket
    {
        public SetMyName Data { get; set; }
        public override void SetPacketType()
        {
            Type = PacketType.SetMyName;
        }
        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandleSetMyName;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SetMyNameResult
    {
        public ushort ErrorCode { get; set; }
    }
    public class SetMyNameResultPacket : ReplyPacket
    {
        public SetMyNameResult Data { get; set; }
        public override void SetPacketType()
        {
            Type = PacketType.SetMyNameResult;
        }
    }

}