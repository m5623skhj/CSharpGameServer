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
    public struct Ping
    {
        public PacketHeader Header;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Pong
    {
        public PacketHeader Header;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CreateRoom
    {
        public PacketHeader Header;
        public string RoomName;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RoomCreated
    {
        public PacketHeader Header;
        public ushort ErrorCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct JoinRoom
    {
        public PacketHeader Header;
        public string RoomName;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RoomJoined
    {
        public PacketHeader Header;
        public ushort ErrorCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LeaveRoom
    {
        public PacketHeader Header;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RoomLeft
    {
        public PacketHeader Header;
        public ushort ErrorCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SendChat
    {
        public PacketHeader Header;
        public string Message;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ChatMessage
    {
        public PacketHeader Header;
        public string Message;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GetRoomList
    {
        public PacketHeader Header;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RoomListUpdate
    {
        public PacketHeader Header;
        public string[] Rooms;
    }

}