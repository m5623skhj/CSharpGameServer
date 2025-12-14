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
        public unsafe fixed byte RoomName[20];
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
        public unsafe fixed byte RoomName[20];
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
        public unsafe fixed byte Message[30];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ChatMessagePacket
    {
        public PacketHeader Header;
        public unsafe fixed byte Message[30];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RoomListRequestPacket
    {
        public PacketHeader Header;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RoomListUpdatePacket
    {
        public PacketHeader Header;
        public unsafe fixed byte Rooms[200];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SetMyNamePacket
    {
        public PacketHeader Header;
        public unsafe fixed byte Name[12];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SetMyNameResultPacket
    {
        public PacketHeader Header;
        public ushort ErrorCode;
    }

}