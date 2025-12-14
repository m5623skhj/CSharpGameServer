using CSharpGameServer.ChattingRoom;
using CSharpGameServer.Packet;
using CSharpGameServer.etc;

namespace ServerUnitTest
{
    public class ChattingRoomUnitTest : IDisposable
    {
        public void Dispose()
        {
            ChattingRoomManager.Instance.InitChattingRoomManager();
        }

        private static CreateRoomPacket CreatePacket(string roomName)
        {
            CreateRoomPacket packet = new();

            unsafe
            {
                fixed (byte* pName = packet.Data.RoomName)
                {
                    FixedStringUtil.Write(roomName, pName, 20);
                }
            }

            return packet;
        }

        public static IEnumerable<object[]> GetAddChattingRoomTestData
        {
            get
            {
                yield return
                [
                    0UL,
                    "TestUser",
                    CreatePacket("TestRoomName")
                ];
            }
        }

        public static IEnumerable<object[]> GetAddChattingRoomInvalidTestData
        {
            get
            {
                yield return
                [
                    0UL,
                    "TestUser",
                    CreatePacket(string.Empty)
                ];

                yield return
                [
                    0UL,
                    "TestUser",
                    CreatePacket("        ")
                ];
            }
        }

        [Theory]
        [MemberData(nameof(GetAddChattingRoomTestData))]
        public void AddChattingRoom_Test(ulong id, string name, CreateRoomPacket packet)
        {
            string roomName;
            unsafe
            {
                fixed (byte* namePointer = packet.Data.RoomName)
                {
                    roomName = FixedStringUtil.Read(namePointer, 20);
                }
            }

            ChattingRoomManager.Instance.AddChattingRoom(id, name, roomName);
            Assert.True(ChattingRoomManager.Instance.ExistsChattingRoom(roomName));
        }

        [Theory]
        [MemberData(nameof(GetAddChattingRoomInvalidTestData))]
        public void AddChattingRoom_Invalid_Test(ulong id, string name, CreateRoomPacket packet)
        {
            var roomName = "";
            unsafe
            {
                fixed (byte* namePointer = packet.Data.RoomName)
                {
                    roomName = FixedStringUtil.Read(namePointer, 20);
                }
            }

            ChattingRoomManager.Instance.AddChattingRoom(id, name, roomName);
            Assert.False(ChattingRoomManager.Instance.ExistsChattingRoom(roomName));
        }

        [Theory]
        [MemberData(nameof(GetAddChattingRoomTestData))]
        public void RemoveChattingRoom_Test(ulong id, string name, CreateRoomPacket packet)
        {
            string roomName;
            unsafe
            {
                fixed (byte* namePointer = packet.Data.RoomName)
                {
                    roomName = FixedStringUtil.Read(namePointer, 20);
                }
            }

            ChattingRoomManager.Instance.OnEnterUser(0);
            Assert.Equal(ErrorCode.Success, ChattingRoomManager.Instance.AddChattingRoom(id, name, roomName));
            ChattingRoomManager.Instance.LeaveChattingRoom(0);
            Assert.False(ChattingRoomManager.Instance.ExistsChattingRoom(roomName));
        }

        [Fact]
        public void GetAllChattingRoom_Test()
        {
            ChattingRoomManager.Instance.OnEnterUser(0);
            ChattingRoomManager.Instance.OnEnterUser(1);

            Assert.Equal(ErrorCode.Success, ChattingRoomManager.Instance.AddChattingRoom(0, "User0", "Room0"));
            Assert.Equal(ErrorCode.Success, ChattingRoomManager.Instance.AddChattingRoom(1, "User1", "Room1"));

            var roomList = ChattingRoomManager.Instance.GetAllChattingRooms();
            Assert.Equal(2, roomList.Count());

            Assert.Equal("Room0", roomList[0]);
            Assert.Equal("Room1", roomList[1]);

            ChattingRoomManager.Instance.LeaveChattingRoom(0);
            ChattingRoomManager.Instance.LeaveChattingRoom(1);

            roomList = ChattingRoomManager.Instance.GetAllChattingRooms();
            Assert.Empty(roomList);
        }
    }
}
