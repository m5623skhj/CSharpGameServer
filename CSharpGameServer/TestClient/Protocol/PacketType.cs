namespace CSharpGameServer
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
        RoomListRequest,
        RoomListUpdate,
        SetMyName,
        SetMyNameResult,
    }
}
