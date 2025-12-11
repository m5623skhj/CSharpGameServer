using CSharpGameServer.Packet;

namespace CSharpGameServer.Core
{
    public partial class Client
    {
        public virtual void HandlePing(Ping ping) { }
        public virtual void HandleCreateRoom(CreateRoom createroom) { }
        public virtual void HandleJoinRoom(JoinRoom joinroom) { }
        public virtual void HandleLeaveRoom(LeaveRoom leaveroom) { }
        public virtual void HandleSendChat(SendChat sendchat) { }
        public virtual void HandleGetRoomList(GetRoomList getroomlist) { }
    }
}