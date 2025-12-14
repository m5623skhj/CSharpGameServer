using CSharpGameServer.etc;
using CSharpGameServer.Packet;
using CSharpGameServer.PacketBase;
using CSharpGameServer.PC;

namespace CSharpGameServer.ChattingRoom
{
    public class ChattingRoomManager
    {
        private static ChattingRoomManager? _instance;
        public static ChattingRoomManager Instance => _instance ??= new ChattingRoomManager();

        private readonly Dictionary<string, ChattingRoom> chattingRooms = [];
        private readonly Lock chattingRoomsLock = new();
        private readonly Dictionary<ulong, string?> memberRoomMapping = [];
        private readonly Lock memberRoomMappingLock = new();
        private readonly HashSet<ulong> lobbyUsers = [];
        private readonly Lock lobbyUsersLock = new();

        private const int RoomNameLengthMax = 20;

        public void OnEnterUser(ulong id)
        {
            lock (lobbyUsersLock)
            {
                lobbyUsers.Add(id);
            }
        }

        public void OnLeaveUser(ulong id)
        {
            lock (lobbyUsersLock)
            {
                if (lobbyUsers.Remove(id))
                {
                    return;
                }
            }

            lock (memberRoomMappingLock)
            {
                if (!memberRoomMapping.TryGetValue(id, out var roomName))
                {
                    return;
                }

                lock (chattingRoomsLock)
                {
                    if (roomName == null || !chattingRooms.TryGetValue(roomName, out var room))
                    {
                        return;
                    }

                    room.RemoveMember(id);
                    memberRoomMapping.Remove(id);
                    if (room.IsEmptyRoom())
                    {
                        chattingRooms.Remove(roomName);
                    }
                }
            }
        }

        public ErrorCode AddChattingRoom(ulong id, string name, string roomName)
        {
            if (string.IsNullOrWhiteSpace(roomName) || roomName.Length > RoomNameLengthMax)
            {
                return ErrorCode.InvalidRoomName;
            }

            lock (memberRoomMappingLock)
            {
                if (!memberRoomMapping.TryAdd(id, roomName))
                {
                    return ErrorCode.AlreadyInRoom;
                }

                lock (chattingRoomsLock)
                {
                    chattingRooms.Add(roomName, new ChattingRoom());
                    chattingRooms[roomName].AddMember(id, name);
                }
            }

            return ErrorCode.Success;
        }

        public void RemoveChattingRoom(string roomName)
        {
            lock (chattingRoomsLock)
            {
                if (chattingRooms.TryGetValue(roomName, out var room))
                {
                    if (!room.IsEmptyRoom())
                    {
                        return;
                    }
                }

                chattingRooms.Remove(roomName, out _);
            }
        }

        public bool ExistsChattingRoom(string roomName)
        {
            lock (memberRoomMappingLock) 
            {
                return chattingRooms.ContainsKey(roomName);
            }
        }

        public int CountChattingRooms()
        {
            lock (chattingRoomsLock)
            {
                return chattingRooms.Count;
            }
        }

        public void InitChattingRoomManager()
        {
            lock (chattingRoomsLock)
            {
                chattingRooms.Clear();
            }

            lock (memberRoomMappingLock)
            {
                memberRoomMapping.Clear();
            }

            lock (lobbyUsersLock)
            {
                lobbyUsers.Clear();
            }
        }

        public ErrorCode JoinChattingRoom(ulong id, string name)
        {
            lock (memberRoomMappingLock)
            {
                if (memberRoomMapping.ContainsKey(id))
                {
                    return ErrorCode.AlreadyInRoom;
                }

                lock (chattingRoomsLock)
                {
                    if (!chattingRooms.TryGetValue(name, out var room))
                    {
                        return ErrorCode.InvalidChatRoom;
                    }

                    if (room.AddMember(id, name))
                    {
                        memberRoomMapping.Add(id, name);
                    }
                }
            }

            lock (lobbyUsersLock)
            {
                lobbyUsers.Remove(id);
            }

            return ErrorCode.Success;
        }

        public void LeaveChattingRoom(ulong id)
        {
            lock (memberRoomMappingLock)
            {
                if (!memberRoomMapping.TryGetValue(id, out var roomName))
                {
                    return;
                }
                lock (chattingRoomsLock)
                {
                    if (roomName == null || !chattingRooms.TryGetValue(roomName, out var room))
                    {
                        return;
                    }

                    room.RemoveMember(id);
                    memberRoomMapping.Remove(id);
                    if (room.IsEmptyRoom())
                    {
                        chattingRooms.Remove(roomName);
                    }
                }
            }

            lock (lobbyUsersLock)
            {
                lobbyUsers.Add(id);
            }

            //PcManager.Instance.FindPc(id)?.Send(new RoomListUpdatePacket()
            //{
            //    Data = new RoomListUpdateData()
            //    {
            //        Rooms = GetAllChattingRooms()
            //    }
            //});
        }

        public void SendChat(ulong id, string message)
        {
            lock (memberRoomMappingLock)
            {
                if (!memberRoomMapping.TryGetValue(id, out var roomName))
                {
                    return;
                }

                lock (chattingRoomsLock)
                {
                    if (roomName != null && chattingRooms.TryGetValue(roomName, out var room))
                    {
                        room.SendMessage(message);
                    }
                }
            }
        }

        public string[] GetAllChattingRooms()
        {
            lock (chattingRoomsLock)
            {
                return chattingRooms.Keys.ToArray();
            }
        }

        private void SendToAllLobbyUsers(ReplyPacket packet)
        {
            lock (lobbyUsersLock)
            {
                foreach (var pc in lobbyUsers.Select(userId => PcManager.Instance.FindPc(userId)))
                {
                    pc?.Send(packet);
                }
            }
        }
    }
}
