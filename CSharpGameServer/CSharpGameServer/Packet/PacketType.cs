namespace CSharpGameServer.Packet
{
    public enum PacketType
    {
        InvalidPacketType,
        Ping,
        Pong,
        CreateRoom,
        RoomCreated,
        JoinRoom,
        RoomJoined,
        LeaveRoom,
        RoomLeft,
        SendChat,
        ChatMessage,
        RoomListUpdate,
        SetMyName,
        SetMyNameResult,
    }
}
