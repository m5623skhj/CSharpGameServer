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

            var roomRemoved = false;
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
                        roomRemoved = true;
                    }
                }
            }

            if (!roomRemoved)
            {
                return;
            }

            var packet = new RoomListUpdatePacket()
            {
                Data = new RoomListUpdate()
                {
                    Rooms = GetAllChattingRooms()
                }
            };
            SendToAllLobbyUsers(packet);
        }

        public void AddChattingRoom(ulong id, string name, CreateRoomPacket packet)
        {
            if (string.IsNullOrWhiteSpace(packet.Data.RoomName) || packet.Data.RoomName.Length > RoomNameLengthMax)
            {
                return;
            }

            lock (memberRoomMappingLock)
            {
                if (memberRoomMapping.ContainsKey(id))
                {
                    return;
                }
                memberRoomMapping.Add(id, packet.Data.RoomName);

                lock (chattingRoomsLock)
                {
                    chattingRooms.Add(packet.Data.RoomName, new ChattingRoom());
                    chattingRooms[packet.Data.RoomName].AddMember(id, name);
                }
            }
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

        public void ClearChattingRooms()
        {
            lock (chattingRoomsLock)
            {
                chattingRooms.Clear();
            }
        }

        public void JoinChattingRoom(ulong id, string name)
        {
            lock (memberRoomMappingLock)
            {
                if (memberRoomMapping.ContainsKey(id))
                {
                    return;
                }

                lock (chattingRoomsLock)
                {
                    if (!chattingRooms.TryGetValue(name, out var room))
                    {
                        return;
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

            PcManager.Instance.FindPc(id)?.Send(new RoomListUpdatePacket()
            {
                Data = new RoomListUpdate()
                {
                    Rooms = GetAllChattingRooms()
                }
            });
        }

        public void SendChat(ulong id, SendChatPacket packet)
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
                        room.SendMessage(packet.Data.Message);
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
