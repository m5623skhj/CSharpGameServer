using CSharpGameServer.Packet;

namespace CSharpGameServer.ChattingRoom
{
    public class ChattingRoomManager
    {
        private static ChattingRoomManager? _instance;
        public static ChattingRoomManager Instance => _instance ??= new ChattingRoomManager();

        private readonly Dictionary<string, ChattingRoom> chattingRooms = [];
        private readonly Lock chattingRoomsLock = new();

        private const int RoomNameLengthMax = 20;

        public void AddChattingRoom(CreateRoomPacket chattingRoom)
        {
            if (string.IsNullOrWhiteSpace(chattingRoom.Data.RoomName) || chattingRoom.Data.RoomName.Length > RoomNameLengthMax)
            {
                return;
            }

            lock (chattingRoomsLock)
            {
                chattingRooms.TryAdd(chattingRoom.Data.RoomName, new ChattingRoom());
            }
        }

        public void RemoveChattingRoom(string roomName)
        {
            lock (chattingRoomsLock)
            {
                chattingRooms.Remove(roomName);
            }
        }

        public bool ExistsChattingRoom(string roomName)
        {
            lock (chattingRoomsLock)
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
    }
}
