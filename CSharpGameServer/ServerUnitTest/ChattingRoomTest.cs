using CSharpGameServer.ChattingRoom;
using CSharpGameServer.Packet;
using CSharpGameServer.etc;

namespace ServerUnitTest
{
    public class ChattingRoomUnitTest : IDisposable
    {
        public void Dispose()
        {
            ChattingRoomManager.Instance.ClearChattingRooms();
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
                    CreatePacket("ThisRoomNameIsTooLongSoItsInvalid")
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
                fixed (byte* pName = packet.Data.RoomName)
                {
                    roomName = FixedStringUtil.Read(pName, 20);
                }
            }

            ChattingRoomManager.Instance.AddChattingRoom(id, name, roomName);
            Assert.True(ChattingRoomManager.Instance.ExistsChattingRoom(roomName));
        }

        [Theory]
        [MemberData(nameof(GetAddChattingRoomInvalidTestData))]
        public void AddChattingRoom_Invalid_Test(ulong id, string name, CreateRoomPacket packet)
        {
            string roomName;
            unsafe
            {
                fixed (byte* pName = packet.Data.RoomName)
                {
                    roomName = FixedStringUtil.Read(pName, 20);
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
                fixed (byte* pName = packet.Data.RoomName)
                {
                    roomName = FixedStringUtil.Read(pName, 20);
                }
            }

            ChattingRoomManager.Instance.AddChattingRoom(id, name, roomName);
            Assert.True(ChattingRoomManager.Instance.ExistsChattingRoom(roomName));
            ChattingRoomManager.Instance.RemoveChattingRoom(roomName);
            Assert.False(ChattingRoomManager.Instance.ExistsChattingRoom(roomName));
        }
    }
}
