using CSharpGameServer.ChattingRoom;
using CSharpGameServer.etc;
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
            string roomName;
            unsafe
            {
                fixed (byte* namePointer = createRoomPacket.Data.RoomName)
                {
                    roomName = FixedStringUtil.Read(namePointer, 20);
                }
            }

            var errorCode = ChattingRoomManager.Instance.AddChattingRoom(ClientSessionId, Name, roomName);
            var resultPacket = new RoomCreatedPacket()
            {
                Data = new RoomCreatedData()
                {
                    ErrorCode = (ushort)errorCode
                }
            };
            Send(resultPacket);
        }

        public override void HandleJoinRoom(JoinRoomPacket joinRoomPacket)
        {
            string roomName;
            unsafe
            {
                fixed (byte* namePointer = joinRoomPacket.Data.RoomName)
                {
                    roomName = FixedStringUtil.Read(namePointer, 20);
                }
            }

            var errorCode = ChattingRoomManager.Instance.JoinChattingRoom(ClientSessionId, roomName);
            var resultPacket = new RoomJoinedPacket()
            {
                Data = new RoomJoinedData()
                {
                    ErrorCode = (ushort)errorCode
                }
            };
            Send(resultPacket);
        }

        public override void HandleLeaveRoom(LeaveRoomPacket leaveRoomPacket)
        {
            ChattingRoomManager.Instance.LeaveChattingRoom(ClientSessionId);
        }

        public override void HandleSendChat(SendChatPacket sendChatPacket)
        {
            string message;
            unsafe
            {
                fixed (byte* messagePointer = sendChatPacket.Data.Message)
                {
                    message = FixedStringUtil.Read(messagePointer, 20);
                }
            }

            ChattingRoomManager.Instance.SendChat(ClientSessionId, message);
        }

        public override void HandleSetMyName(SetMyNamePacket setMyNamePacket)
        {
            string name;
            unsafe
            {
                fixed (byte* namePointer = setMyNamePacket.Data.Name)
                {
                    name = FixedStringUtil.Read(namePointer, 12);
                }
            }

            var errorCode = SetMyName(name);
            var resultPacket = new SetMyNameResultPacket()
            {
                Data = new SetMyNameResultData()
                {
                    ErrorCode = (ushort)errorCode
                }
            };
            Send(resultPacket);
        }

        public override void HandleRoomListRequest(RoomListRequestPacket roomlistrequestpacket)
        {
            var roomList = ChattingRoomManager.Instance.GetAllChattingRooms();
            var roomListPacket = new RoomListUpdatePacket();
            
            unsafe
            {
                fixed (byte* roomsPointer = roomListPacket.Data.Rooms)
                {
                    var offset = 0;
                    foreach (var room in roomList)
                    {
                        FixedStringUtil.Write(room, roomsPointer + offset, 20);
                        offset += 20;

                        if (offset >= 200)
                        {
                            break;
                        }
                    }
                }
            }

            Send(roomListPacket);
        }
    }
}
