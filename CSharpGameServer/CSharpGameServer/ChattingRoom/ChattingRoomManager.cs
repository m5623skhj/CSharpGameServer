using CSharpGameServer.Packet;

namespace CSharpGameServer.ChattingRoom
{
    internal class ChattingRoomManager
    {
        private static ChattingRoomManager? _instance;
        public static ChattingRoomManager Instance => _instance ??= new ChattingRoomManager();

        private readonly Dictionary<string, ChattingRoom> chattingRooms = [];
        private readonly Lock chattingRoomsLock = new();

        public void AddChattingRoom(CreateRoom chattingRoom)
        {
            lock (chattingRoomsLock)
            {
                chattingRooms.TryAdd(chattingRoom.RoomName, new ChattingRoom());
            }
        }

        public void RemoveChattingRoom(string roomName)
        {
            lock (chattingRoomsLock)
            {
                chattingRooms.Remove(roomName);
            }
        }
    }
}
