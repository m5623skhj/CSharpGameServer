using CSharpGameServer.ChattingRoom;
using CSharpGameServer.Packet;

namespace CSharpGameServer.PC
{
    public partial class Pc
    {
        public override void HandlePing(PingPacket ping)
        {
            var pong = new PongPacket();
            Send(pong);
        }

        public override void HandleCreateRoom(CreateRoomPacket createRoomPacket)
        {
            ChattingRoomManager.Instance.AddChattingRoom(ClientSessionId, Name, createRoomPacket);
        }

        public override void HandleJoinRoom(JoinRoomPacket joinRoomPacket)
        {
            ChattingRoomManager.Instance.JoinChattingRoom(ClientSessionId, Name);
        }

        public override void HandleLeaveRoom(LeaveRoomPacket leaveRoomPacket)
        {
            ChattingRoomManager.Instance.LeaveChattingRoom(ClientSessionId);
        }

        public override void HandleSendChat(SendChatPacket sendChatPacket)
        {
            ChattingRoomManager.Instance.SendChat(ClientSessionId, sendChatPacket);
        }

        public override void HandleSetMyName(SetMyNamePacket setMyNamePacket)
        {
            var errorCode = SetMyName(setMyNamePacket.Data.Name);
            var resultPacket = new SetMyNameResultPacket()
            {
                Data = new SetMyNameResult()
                {
                    ErrorCode = (ushort)errorCode
                }
            };
            Send(resultPacket);
        }
    }
}
