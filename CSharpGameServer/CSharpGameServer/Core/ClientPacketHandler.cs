using CSharpGameServer.Packet;

namespace CSharpGameServer.Core
{
    public partial class Client
    {
        public virtual void HandlePing(PingPacket pingpacket) { }
        public virtual void HandleCreateRoom(CreateRoomPacket createroompacket) { }
        public virtual void HandleJoinRoom(JoinRoomPacket joinroompacket) { }
        public virtual void HandleLeaveRoom(LeaveRoomPacket leaveroompacket) { }
        public virtual void HandleSendChat(SendChatPacket sendchatpacket) { }
        public virtual void HandleRoomListRequest(RoomListRequestPacket roomlistrequestpacket) { }
        public virtual void HandleSetMyName(SetMyNamePacket setmynamepacket) { }
    }
}