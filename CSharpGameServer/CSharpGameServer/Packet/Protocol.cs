using CSharpGameServer.Core;
using CSharpGameServer.PacketBase;
using System.Runtime.InteropServices;

namespace CSharpGameServer.Packet
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Ping : RequestPacket
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

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Pong : ReplyPacket
    {
        public override void SetPacketType()
        {
            Type = PacketType.Pong;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class CreateRoom : RequestPacket
    {
        public override void SetPacketType()
        {
            Type = PacketType.CreateRoom;
        }
        public string RoomName { get; set; }
        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandleCreateRoom;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class RoomCreated : ReplyPacket
    {
        public override void SetPacketType()
        {
            Type = PacketType.RoomCreated;
        }
        public ushort ErrorCode { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class JoinRoom : RequestPacket
    {
        public override void SetPacketType()
        {
            Type = PacketType.JoinRoom;
        }
        public string RoomName { get; set; }
        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandleJoinRoom;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class RoomJoined : ReplyPacket
    {
        public override void SetPacketType()
        {
            Type = PacketType.RoomJoined;
        }
        public ushort ErrorCode { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class LeaveRoom : RequestPacket
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
    public class RoomLeft : ReplyPacket
    {
        public override void SetPacketType()
        {
            Type = PacketType.RoomLeft;
        }
        public ushort ErrorCode { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class SendChat : RequestPacket
    {
        public override void SetPacketType()
        {
            Type = PacketType.SendChat;
        }
        public string Message { get; set; }
        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandleSendChat;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class ChatMessage : ReplyPacket
    {
        public override void SetPacketType()
        {
            Type = PacketType.ChatMessage;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class GetRoomList : RequestPacket
    {
        public override void SetPacketType()
        {
            Type = PacketType.GetRoomList;
        }
        protected override Action<Client, RequestPacket> GetHandler()
        {
            return PacketHandlerManager.HandleGetRoomList;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class RoomListUpdate : ReplyPacket
    {
        public override void SetPacketType()
        {
            Type = PacketType.RoomListUpdate;
        }
        public string[] Rooms { get; set; }
    }

}