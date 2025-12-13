using CSharpGameServer.Packet;
using System.Collections.Concurrent;

namespace CSharpGameServer.ChattingRoom
{
    public class ChattingRoomManager
    {
        private static ChattingRoomManager? _instance;
        public static ChattingRoomManager Instance => _instance ??= new ChattingRoomManager();

        private readonly ConcurrentDictionary<string, ChattingRoom> chattingRooms = [];

        private const int RoomNameLengthMax = 20;

        public void AddChattingRoom(CreateRoomPacket chattingRoom)
        {
            if (string.IsNullOrWhiteSpace(chattingRoom.Data.RoomName) || chattingRoom.Data.RoomName.Length > RoomNameLengthMax)
            {
                return;
            }
            
            chattingRooms.TryAdd(chattingRoom.Data.RoomName, new ChattingRoom());
        }

        public void RemoveChattingRoom(string roomName)
        {
            chattingRooms.Remove(roomName, out _);
        }

        public bool ExistsChattingRoom(string roomName)
        {
            return chattingRooms.ContainsKey(roomName);
        }

        public int CountChattingRooms()
        {
            return chattingRooms.Count;
        }

        public void ClearChattingRooms()
        {
            chattingRooms.Clear();
        }
    }
}
