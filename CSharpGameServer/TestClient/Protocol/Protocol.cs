using System.Runtime.InteropServices;

namespace CSharpGameServer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketHeader
    {
        public int PacketType;
        public ushort PacketSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PingPacket
    {
        public PacketHeader Header;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PongPacket
    {
        public PacketHeader Header;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CreateRoomPacket
    {
        public PacketHeader Header;
        public string RoomName;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RoomCreatedPacket
    {
        public PacketHeader Header;
        public ushort ErrorCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct JoinRoomPacket
    {
        public PacketHeader Header;
        public string RoomName;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RoomJoinedPacket
    {
        public PacketHeader Header;
        public ushort ErrorCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LeaveRoomPacket
    {
        public PacketHeader Header;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RoomLeftPacket
    {
        public PacketHeader Header;
        public ushort ErrorCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SendChatPacket
    {
        public PacketHeader Header;
        public string Message;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ChatMessagePacket
    {
        public PacketHeader Header;
        public string Message;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RoomListUpdatePacket
    {
        public PacketHeader Header;
        public string[] Rooms;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SetMyNamePacket
    {
        public PacketHeader Header;
        public string Name;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SetMyNameResultPacket
    {
        public PacketHeader Header;
        public ushort ErrorCode;
    }

}